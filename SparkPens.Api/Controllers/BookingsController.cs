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

        // POST: api/bookings (Untuk Mahasiswa Booking)
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto dto)
        {
            // Validasi apakah ruangan ada
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null) return BadRequest("Ruangan tidak ditemukan.");

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
