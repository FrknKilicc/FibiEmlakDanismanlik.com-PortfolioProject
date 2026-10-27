using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingDeals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListingDeals",
                columns: table => new
                {
                    ListingDealId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingTypeId = table.Column<int>(type: "int", nullable: false),
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    DealType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AgreedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DepositDate = table.Column<DateTime>(type: "date", nullable: true),
                    BuyerCommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    SellerCommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    BuyerCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SellerCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DepositOutcome = table.Column<int>(type: "int", nullable: true),
                    DepositRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DepositRefundDate = table.Column<DateTime>(type: "date", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "date", nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "date", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedByAgentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingDeals", x => x.ListingDealId);
                    table.ForeignKey(
                        name: "FK_ListingDeals_Agents_CreatedByAgentId",
                        column: x => x.CreatedByAgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DealShares",
                columns: table => new
                {
                    DealShareId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingDealId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    RoleAtDeal = table.Column<int>(type: "int", nullable: false),
                    SharePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PaidDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealShares", x => x.DealShareId);
                    table.ForeignKey(
                        name: "FK_DealShares_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealShares_ListingDeals_ListingDealId",
                        column: x => x.ListingDealId,
                        principalTable: "ListingDeals",
                        principalColumn: "ListingDealId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealShares_AgentId_PaymentStatus",
                table: "DealShares",
                columns: new[] { "AgentId", "PaymentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_DealShares_ListingDealId_AgentId",
                table: "DealShares",
                columns: new[] { "ListingDealId", "AgentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingDeals_CreatedByAgentId",
                table: "ListingDeals",
                column: "CreatedByAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingDeals_ListingTypeId_ListingId",
                table: "ListingDeals",
                columns: new[] { "ListingTypeId", "ListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_ListingDeals_Status_CompletedDate",
                table: "ListingDeals",
                columns: new[] { "Status", "CompletedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealShares");

            migrationBuilder.DropTable(
                name: "ListingDeals");
        }
    }
}
