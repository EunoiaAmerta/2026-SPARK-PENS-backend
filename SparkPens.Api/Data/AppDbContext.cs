using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Models;

namespace SparkPens.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Ini akan jadi tabel "Bookings" di database
    public DbSet<Booking> Bookings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Menambahkan data contoh (Seeding)
        modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                // Ganti Guid.NewGuid() dengan string GUID statis
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                NIM = "21102030",
                NamaMahasiswa = "Kratos Spartan",
                NamaRuangan = "Lab Programming 1",
                // Gunakan tanggal statis (Contoh: 10 Feb 2026 jam 10 pagi)
                WaktuMulai = new DateTime(2026, 2, 10, 10, 0, 0, DateTimeKind.Utc),
                WaktuSelesai = new DateTime(2026, 2, 10, 12, 0, 0, DateTimeKind.Utc),
                Keperluan = "Latihan ASP.NET Core",
                Status = "Approved"
            }
        );
    }
}