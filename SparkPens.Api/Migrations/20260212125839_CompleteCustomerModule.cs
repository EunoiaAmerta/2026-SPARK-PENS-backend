using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SparkPens.Api.Migrations
{
    /// <inheritdoc />
    public partial class CompleteCustomerModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "CreatedDate", "DeletedAt", "Email", "IsDeleted", "Name", "Phone", "Status" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "Jl. Raya Kampus PENS No. 1", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, "budi.santoso@pens.ac.id", false, "Budi Santoso", "081234567890", "active" },
                    { new Guid("b2c3d4e5-f6a1-4b6c-9d0e-1f2a3b4c5d6e"), "Griya Gebang Blok C-12", new DateTime(2026, 2, 2, 11, 30, 0, 0, DateTimeKind.Utc), null, "siti.aminah@pens.ac.id", false, "Siti Aminah", "081234567891", "active" },
                    { new Guid("c3d4e5f6-a1b2-4c7d-0e1f-2a3b4c5d6e7f"), "Keputih Tegal Gg. III", new DateTime(2026, 2, 3, 9, 15, 0, 0, DateTimeKind.Utc), null, "andi.wijaya@pens.ac.id", false, "Andi Wijaya", "081234567892", "inactive" },
                    { new Guid("d4e5f6a1-b2c3-4d8e-1f2a-3b4c5d6e7f8a"), "Mulyosari Permai No. 45", new DateTime(2026, 2, 4, 14, 20, 0, 0, DateTimeKind.Utc), null, "rina.rose@pens.ac.id", false, "Rina Rose", "081234567893", "active" },
                    { new Guid("e5f6a1b2-c3d4-4e9f-2a3b-4c5d6e7f8a9b"), "Asrama Mahasiswa PENS", new DateTime(2026, 2, 5, 16, 45, 0, 0, DateTimeKind.Utc), null, "eko.prasetyo@pens.ac.id", false, "Eko Prasetyo", "081234567894", "active" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "Keperluan", "NIM", "NamaMahasiswa", "NamaRuangan", "Status", "WaktuMulai", "WaktuSelesai" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "Latihan ASP.NET Core", "21102030", "Kratos Spartan", "Lab Programming 1", "Approved", new DateTime(2026, 2, 10, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 10, 12, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
