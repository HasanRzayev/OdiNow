using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdiNow.Migrations
{
    /// <inheritdoc />
    public partial class UserTicketsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketClaims");

            migrationBuilder.DropTable(
                name: "TicketDrops");

            migrationBuilder.CreateTable(
                name: "UserTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTickets_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTickets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTickets_OfferId",
                table: "UserTickets",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTickets_UserId_Status",
                table: "UserTickets",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTickets");

            migrationBuilder.CreateTable(
                name: "TicketDrops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TicketsRemaining = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TicketsTotal = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketDrops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketDrops_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketDropId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ExpiredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    QrPayload = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RedeemedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketClaims_TicketDrops_TicketDropId",
                        column: x => x.TicketDropId,
                        principalTable: "TicketDrops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketClaims_Code",
                table: "TicketClaims",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketClaims_TicketDropId",
                table: "TicketClaims",
                column: "TicketDropId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketClaims_UserId_TicketDropId",
                table: "TicketClaims",
                columns: new[] { "UserId", "TicketDropId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketDrops_IsActive_AvailableFrom",
                table: "TicketDrops",
                columns: new[] { "IsActive", "AvailableFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketDrops_OfferId",
                table: "TicketDrops",
                column: "OfferId");
        }
    }
}
