using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mottu.Rentals.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Couriers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CnhNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CnhType = table.Column<int>(type: "integer", nullable: false),
                    CnhImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Couriers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motorcycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Plate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motorcycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YearNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MotorcycleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourierId = table.Column<Guid>(type: "uuid", nullable: false),
                    MotorcycleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanDays = table.Column<int>(type: "integer", nullable: false),
                    DailyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Couriers_CourierId",
                        column: x => x.CourierId,
                        principalTable: "Couriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Motorcycles_MotorcycleId",
                        column: x => x.MotorcycleId,
                        principalTable: "Motorcycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Couriers_CnhNumber",
                table: "Couriers",
                column: "CnhNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Couriers_Cnpj",
                table: "Couriers",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycles_Plate",
                table: "Motorcycles",
                column: "Plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CourierId",
                table: "Rentals",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_MotorcycleId",
                table: "Rentals",
                column: "MotorcycleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "YearNotifications");

            migrationBuilder.DropTable(
                name: "Couriers");

            migrationBuilder.DropTable(
                name: "Motorcycles");
        }
    }
}
