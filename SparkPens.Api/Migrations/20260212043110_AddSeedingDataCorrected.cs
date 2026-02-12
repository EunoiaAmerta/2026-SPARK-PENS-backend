using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SparkPens.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedingDataCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("5715d065-904d-45d2-83a2-19ae9bcc3dcd"));

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "Keperluan", "NIM", "NamaMahasiswa", "NamaRuangan", "Status", "WaktuMulai", "WaktuSelesai" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "Latihan ASP.NET Core", "21102030", "Kratos Spartan", "Lab Programming 1", "Approved", new DateTime(2026, 2, 10, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 10, 12, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "Keperluan", "NIM", "NamaMahasiswa", "NamaRuangan", "Status", "WaktuMulai", "WaktuSelesai" },
                values: new object[] { new Guid("5715d065-904d-45d2-83a2-19ae9bcc3dcd"), "Latihan ASP.NET Core", "21102030", "Kratos Spartan", "Lab Programming 1", "Approved", new DateTime(2026, 2, 12, 2, 13, 52, 558, DateTimeKind.Utc).AddTicks(4095), new DateTime(2026, 2, 12, 4, 13, 52, 558, DateTimeKind.Utc).AddTicks(4231) });
        }
    }
}
