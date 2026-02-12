using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Models;

namespace SparkPens.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Ini akan jadi tabel "Bookings" di database
    public DbSet<Booking> Bookings { get;set; }
    public DbSet<Customer> Customers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Menambahkan data contoh (Seeding)
        modelBuilder.Entity<Customer>().HasData(
            new Customer 
            { 
                Id = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), 
                Name = "Budi Santoso", 
                Email = "budi.santoso@pens.ac.id", 
                Phone = "081234567890", 
                Address = "Jl. Raya Kampus PENS No. 1", 
                Status = "active", 
                CreatedDate = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false 
            },
            new Customer 
            { 
                Id = Guid.Parse("b2c3d4e5-f6a1-4b6c-9d0e-1f2a3b4c5d6e"), 
                Name = "Siti Aminah", 
                Email = "siti.aminah@pens.ac.id", 
                Phone = "081234567891", 
                Address = "Griya Gebang Blok C-12", 
                Status = "active", 
                CreatedDate = new DateTime(2026, 2, 2, 11, 30, 0, DateTimeKind.Utc),
                IsDeleted = false 
            },
            new Customer 
            { 
                Id = Guid.Parse("c3d4e5f6-a1b2-4c7d-0e1f-2a3b4c5d6e7f"), 
                Name = "Andi Wijaya", 
                Email = "andi.wijaya@pens.ac.id", 
                Phone = "081234567892", 
                Address = "Keputih Tegal Gg. III", 
                Status = "inactive", 
                CreatedDate = new DateTime(2026, 2, 3, 09, 15, 0, DateTimeKind.Utc),
                IsDeleted = false 
            },
            new Customer 
            { 
                Id = Guid.Parse("d4e5f6a1-b2c3-4d8e-1f2a-3b4c5d6e7f8a"), 
                Name = "Rina Rose", 
                Email = "rina.rose@pens.ac.id", 
                Phone = "081234567893", 
                Address = "Mulyosari Permai No. 45", 
                Status = "active", 
                CreatedDate = new DateTime(2026, 2, 4, 14, 20, 0, DateTimeKind.Utc),
                IsDeleted = false 
            },
            new Customer 
            { 
                Id = Guid.Parse("e5f6a1b2-c3d4-4e9f-2a3b-4c5d6e7f8a9b"), 
                Name = "Eko Prasetyo", 
                Email = "eko.prasetyo@pens.ac.id", 
                Phone = "081234567894", 
                Address = "Asrama Mahasiswa PENS", 
                Status = "active", 
                CreatedDate = new DateTime(2026, 2, 5, 16, 45, 0, DateTimeKind.Utc),
                IsDeleted = false 
            }
        );
    }
}