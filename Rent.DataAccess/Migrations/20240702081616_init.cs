using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleClientHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleClientHistories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "VehicleClientHistories",
                columns: new[] { "Id", "ClientId", "EndDate", "StartDate", "VehicleId" },
                values: new object[,]
                {
                    { new Guid("26668b34-c23a-46a2-a2a3-1034671acacb"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2023, 9, 23, 20, 42, 0, 500, DateTimeKind.Utc).AddTicks(8584), new DateTime(2023, 8, 28, 11, 18, 12, 9, DateTimeKind.Utc).AddTicks(6795), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("55d70304-13c3-4d22-b6ac-42859a23f912"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2024, 1, 4, 16, 24, 29, 242, DateTimeKind.Utc).AddTicks(6142), new DateTime(2024, 3, 22, 7, 38, 0, 405, DateTimeKind.Utc).AddTicks(1235), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("a9387e8d-ba91-4f87-9fc8-cc8309515a12"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2024, 6, 25, 21, 36, 41, 479, DateTimeKind.Utc).AddTicks(4918), new DateTime(2023, 11, 23, 14, 4, 2, 653, DateTimeKind.Utc).AddTicks(4315), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("c6c33095-990c-4593-a28d-d6f98fc068bb"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2023, 9, 7, 0, 32, 12, 359, DateTimeKind.Utc).AddTicks(1459), new DateTime(2024, 3, 21, 21, 16, 55, 891, DateTimeKind.Utc).AddTicks(113), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("d455df39-0d7f-4e16-846d-8e3b1e84909b"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2024, 4, 25, 11, 13, 45, 634, DateTimeKind.Utc).AddTicks(8015), new DateTime(2023, 10, 21, 8, 14, 17, 577, DateTimeKind.Utc).AddTicks(5577), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleClientHistories");
        }
    }
}
