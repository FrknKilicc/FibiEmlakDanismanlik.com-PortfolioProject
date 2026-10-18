using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FibiServicesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankLoanRates",
                columns: table => new
                {
                    BankLoanRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HousingLoanMonthlyRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    CommercialLoanMonthlyRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    MaxTermMonths = table.Column<int>(type: "int", nullable: false),
                    AllocationFeeRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankLoanRates", x => x.BankLoanRateId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalNeighborhoodMarketStats",
                columns: table => new
                {
                    ExternalNeighborhoodMarketStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NeighborhoodId = table.Column<int>(type: "int", nullable: false),
                    BaseSalePricePerSqm = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BaseRentPricePerSqm = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AgencyMarginMultiplier = table.Column<decimal>(type: "decimal(5,3)", precision: 5, scale: 3, nullable: false, defaultValue: 1.08m),
                    BandSpreadPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 15m),
                    Last1YearGrowthRate = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    Last3YearsCompoundGrowthRate = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    RegionalDevelopmentFactor = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    SoilRiskClass = table.Column<int>(type: "int", nullable: false),
                    UrbanTransformationAppetiteScore = table.Column<int>(type: "int", nullable: false),
                    HistoricalQuarterlyPricesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalNeighborhoodMarketStats", x => x.ExternalNeighborhoodMarketStatId);
                    table.ForeignKey(
                        name: "FK_ExternalNeighborhoodMarketStats_Neighborhoods_NeighborhoodId",
                        column: x => x.NeighborhoodId,
                        principalTable: "Neighborhoods",
                        principalColumn: "NeighborhoodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FibiServiceLeads",
                columns: table => new
                {
                    FibiServiceLeadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceModule = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NeighborhoodId = table.Column<int>(type: "int", nullable: true),
                    CalculationSummaryJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsContacted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FibiServiceLeads", x => x.FibiServiceLeadId);
                    table.ForeignKey(
                        name: "FK_FibiServiceLeads_Neighborhoods_NeighborhoodId",
                        column: x => x.NeighborhoodId,
                        principalTable: "Neighborhoods",
                        principalColumn: "NeighborhoodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BankLoanRates",
                columns: new[] { "BankLoanRateId", "AllocationFeeRate", "BankName", "CommercialLoanMonthlyRate", "HousingLoanMonthlyRate", "IsActive", "LogoUrl", "MaxTermMonths", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 0.50m, "Ziraat Bankası", 2.89m, 2.49m, true, null, 120, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 0.50m, "Halkbank", 2.95m, 2.52m, true, null, 120, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 0.50m, "VakıfBank", 2.99m, 2.55m, true, null, 120, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 0.50m, "Türkiye İş Bankası", 3.19m, 2.79m, true, null, 120, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 0.50m, "Garanti BBVA", 3.25m, 2.85m, true, null, 120, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalNeighborhoodMarketStats_NeighborhoodId",
                table: "ExternalNeighborhoodMarketStats",
                column: "NeighborhoodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FibiServiceLeads_IsContacted_CreatedDate",
                table: "FibiServiceLeads",
                columns: new[] { "IsContacted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FibiServiceLeads_NeighborhoodId",
                table: "FibiServiceLeads",
                column: "NeighborhoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankLoanRates");

            migrationBuilder.DropTable(
                name: "ExternalNeighborhoodMarketStats");

            migrationBuilder.DropTable(
                name: "FibiServiceLeads");
        }
    }
}
