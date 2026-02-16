using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Models;

namespace SparkPens.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Daftar Tabel Database
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi Tabel Room
            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Rooms"); // Memastikan nama tabel di PostgreSQL
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                
                // Filter otomatis agar data yang isDeleted = true tidak muncul
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Konfigurasi Tabel Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequesterName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                
                // Relasi: Satu Room bisa memiliki banyak Booking
                entity.HasOne(d => d.Room)
                    .WithMany()
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}