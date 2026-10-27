using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListingAlerts",
                columns: table => new
                {
                    ListingAlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ListingTypeId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    MinPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RoomCounts = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourceListingTypeId = table.Column<int>(type: "int", nullable: true),
                    SourceListingId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AssignedAgentId = table.Column<int>(type: "int", nullable: true),
                    KvkkApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAlerts", x => x.ListingAlertId);
                    table.ForeignKey(
                        name: "FK_ListingAlerts_Agents_AssignedAgentId",
                        column: x => x.AssignedAgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListingAlerts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListingAlerts_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListingAlertMatches",
                columns: table => new
                {
                    ListingAlertMatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingAlertId = table.Column<int>(type: "int", nullable: false),
                    ListingTypeId = table.Column<int>(type: "int", nullable: false),
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAlertMatches", x => x.ListingAlertMatchId);
                    table.ForeignKey(
                        name: "FK_ListingAlertMatches_ListingAlerts_ListingAlertId",
                        column: x => x.ListingAlertId,
                        principalTable: "ListingAlerts",
                        principalColumn: "ListingAlertId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlertMatches_ListingAlertId_ListingTypeId_ListingId",
                table: "ListingAlertMatches",
                columns: new[] { "ListingAlertId", "ListingTypeId", "ListingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlertMatches_ListingTypeId_ListingId",
                table: "ListingAlertMatches",
                columns: new[] { "ListingTypeId", "ListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlerts_AssignedAgentId",
                table: "ListingAlerts",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlerts_CityId",
                table: "ListingAlerts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlerts_DistrictId",
                table: "ListingAlerts",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAlerts_Status_ListingTypeId_CityId",
                table: "ListingAlerts",
                columns: new[] { "Status", "ListingTypeId", "CityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingAlertMatches");

            migrationBuilder.DropTable(
                name: "ListingAlerts");
        }
    }
}
