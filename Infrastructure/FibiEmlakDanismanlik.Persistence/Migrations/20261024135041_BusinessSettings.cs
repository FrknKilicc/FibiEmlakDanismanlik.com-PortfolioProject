using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BusinessSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessSettings",
                columns: table => new
                {
                    BusinessSettingId = table.Column<int>(type: "int", nullable: false),
                    SaleBuyerCommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SaleSellerCommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RentCommissionMonths = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    BestDealMaxPerUsageType = table.Column<int>(type: "int", nullable: false),
                    ListingAlertMaxActivePerPhone = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByAgentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSettings", x => x.BusinessSettingId);
                });

            migrationBuilder.InsertData(
                table: "BusinessSettings",
                columns: new[] { "BusinessSettingId", "BestDealMaxPerUsageType", "ListingAlertMaxActivePerPhone", "RentCommissionMonths", "SaleBuyerCommissionRate", "SaleSellerCommissionRate", "UpdatedAtUtc", "UpdatedByAgentId" },
                values: new object[] { 1, 5, 5, 1m, 2m, 2m, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessSettings");
        }
    }
}
