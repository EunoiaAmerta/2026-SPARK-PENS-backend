using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Data;
using SparkPens.Api.DTOs;
using SparkPens.Api.Models;

namespace SparkPens.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    // 1. READ: Ambil semua data peminjaman
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
    {
        return await _context.Bookings.ToListAsync();
    }

    // 2. READ: Ambil satu data berdasarkan ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null) return NotFound();

        return booking;
    }

    // 3. CREATE: Tambah peminjaman baru (Menggunakan DTO)
    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking(BookingDto dto)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            NIM = dto.NIM,
            NamaMahasiswa = dto.NamaMahasiswa,
            NamaRuangan = dto.NamaRuangan,
            WaktuMulai = DateTime.SpecifyKind(dto.WaktuMulai, DateTimeKind.Utc),
            WaktuSelesai = DateTime.SpecifyKind(dto.WaktuSelesai, DateTimeKind.Utc),
            Keperluan = dto.Keperluan,
            Status = "Pending"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    // 4. DELETE: Hapus data peminjaman
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}