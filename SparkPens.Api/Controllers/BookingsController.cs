using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Data;
using SparkPens.Api.DTOs;
using SparkPens.Api.Models;

namespace SparkPens.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            // Include("Room") agar nama ruangan ikut terambil saat ditampilkan di Admin Panel
            return await _context.Bookings
                .Include(b => b.Room) 
                .OrderByDescending(b => b.CreatedDate) // Urutkan dari yang terbaru
                .ToListAsync();
        }

        // GET: api/bookings/room/{roomId}?date={date}
        // Untuk mendapatkan booking berdasarkan ruangan dan tanggal
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByRoom(
            Guid roomId, 
            [FromQuery] string? date = null)
        {
            // Use AsNoTracking and IgnoreQueryFilters for consistency
            var query = _context.Bookings
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(b => b.Room)
                .Where(b => b.RoomId == roomId && !b.IsDeleted);

            // Filter berdasarkan tanggal jika disediakan
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out var filterDate))
                {
                    // Konversi ke UTC untuk PostgreSQL
                    var startOfDay = DateTime.SpecifyKind(filterDate.Date, DateTimeKind.Utc);
                    var endOfDay = DateTime.SpecifyKind(filterDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                    
                    Console.WriteLine($"GetBookingsByRoom: Filtering for {startOfDay} to {endOfDay}");
                    
                    query = query.Where(b => 
                        (b.BookingStartDate >= startOfDay && b.BookingStartDate < endOfDay) ||
                        (b.BookingEndDate >= startOfDay && b.BookingEndDate < endOfDay) ||
                        (b.BookingStartDate <= startOfDay && b.BookingEndDate >= endOfDay));
                }
            }

            // Hanya ambil booking dengan status Pending atau Approved
            query = query.Where(b => b.Status == "Pending" || b.Status == "Approved");

            var result = await query.OrderBy(b => b.BookingStartDate).ToListAsync();
            Console.WriteLine($"GetBookingsByRoom: Found {result.Count} bookings for room {roomId}");
            
            return result;
        }

        // POST: api/bookings (Untuk Mahasiswa Booking)
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto dto)
        {
            // Validasi apakah ruangan ada
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room == null) return BadRequest("Ruangan tidak ditemukan.");

            // Validasi overlapping booking
            var newStart = dto.BookingStartDate.ToUniversalTime();
            var newEnd = dto.BookingEndDate.ToUniversalTime();

            // Debug: Log the dates
            Console.WriteLine($"New booking: {newStart} to {newEnd} for room {dto.RoomId}");

            // Cek apakah ada booking yang overlap (status Pending atau Approved)
            // Gunakan AsNoTracking() untuk bypass query filter dan ignore autoIncludes
            var existingBookings = await _context.Bookings
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(b => b.RoomId == dto.RoomId && !b.IsDeleted)
                .Where(b => b.Status == "Pending" || b.Status == "Approved")
                .ToListAsync();

            Console.WriteLine($"Found {existingBookings.Count} existing bookings for this room");

            // Check for overlap
            var overlappingBookings = existingBookings
                .Where(b => newStart < b.BookingEndDate && newEnd > b.BookingStartDate)
                .Select(b => new {
                    b.BookingStartDate,
                    b.BookingEndDate,
                    b.RequesterName,
                    b.Status
                })
                .ToList();

            Console.WriteLine($"Found {overlappingBookings.Count} overlapping bookings");

            if (overlappingBookings.Any())
            {
                var conflictDetails = string.Join(", ", overlappingBookings.Select(b => 
                    $"{b.BookingStartDate:dd/MM/yyyy HH:mm} - {b.BookingEndDate:dd/MM/yyyy HH:mm} ({b.RequesterName}, {b.Status})"));
                
                return BadRequest(new { 
                    message = "Ruangan sudah dipinjam pada waktu tersebut!", 
                    conflicts = overlappingBookings 
                });
            }

            var booking = new Booking
            {
                RoomId = dto.RoomId,
                RequesterName = dto.RequesterName,
                RequesterEmail = dto.RequesterEmail,
                RequesterPhone = dto.RequesterPhone,
                BookingStartDate = dto.BookingStartDate,
                BookingEndDate = dto.BookingEndDate,
                Purpose = dto.Purpose,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookings", new { id = booking.Id }, booking);
        }

        // PATCH: api/bookings/{id}/status (Untuk Admin ACC/Reject)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Booking tidak ditemukan.");

            // Validasi input status agar tidak sembarangan string
            if (dto.Status != "Approved" && dto.Status != "Rejected" && dto.Status != "Pending")
            {
                return BadRequest("Status tidak valid. Gunakan: Approved, Rejected, atau Pending.");
            }

            booking.Status = dto.Status;
            
            // Simpan alasan penolakan jika ada
            if (!string.IsNullOrEmpty(dto.RejectionReason))
            {
                // TODO: Kirim email ke peminjam dengan alasan penolakan
                // EmailService.Send(booking.RequesterEmail, "Booking Ditolak", dto.RejectionReason);
            }
            
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Status berhasil diubah menjadi {dto.Status}", data = booking });
        }

        // DELETE: api/bookings/{id} (Untuk hapus booking)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Booking tidak ditemukan.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking berhasil dihapus" });
        }
    }
}
