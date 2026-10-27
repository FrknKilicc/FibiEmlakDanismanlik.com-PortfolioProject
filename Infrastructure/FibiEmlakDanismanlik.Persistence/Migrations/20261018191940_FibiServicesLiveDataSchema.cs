using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FibiServicesLiveDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KvkkApprovedAt",
                table: "FibiServiceLeads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE FibiServiceLeads SET KvkkApprovedAt = CreatedDate;");

            migrationBuilder.AddColumn<string>(
                name: "DataSource",
                table: "BankLoanRates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderKey",
                table: "BankLoanRates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "BankLoanRates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalNeighborhoodMarketStats_Neighborhoods_NeighborhoodId",
                table: "ExternalNeighborhoodMarketStats");

            migrationBuilder.DropIndex(
                name: "IX_ExternalNeighborhoodMarketStats_NeighborhoodId",
                table: "ExternalNeighborhoodMarketStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalNeighborhoodMarketStats",
                table: "ExternalNeighborhoodMarketStats");

            migrationBuilder.RenameTable(
                name: "ExternalNeighborhoodMarketStats",
                newName: "ExternalMarketStats");

            migrationBuilder.RenameColumn(
                name: "ExternalNeighborhoodMarketStatId",
                table: "ExternalMarketStats",
                newName: "ExternalMarketStatId");

            migrationBuilder.AlterColumn<int>(
                name: "NeighborhoodId",
                table: "ExternalMarketStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "SoilRiskClass",
                table: "ExternalMarketStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "ExternalMarketStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "ExternalMarketStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "ExternalMarketStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSource",
                table: "ExternalMarketStats",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastAppliedIndexPeriod",
                table: "ExternalMarketStats",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE s SET s.Scope = 4, s.DistrictId = n.DistrictId, s.CityId = d.CityId, s.DataSource = N'FİBİ tahmini başlangıç verisi'
FROM ExternalMarketStats s
INNER JOIN Neighborhoods n ON n.NeighborhoodId = s.NeighborhoodId
INNER JOIN Districts d ON d.DistrictId = n.DistrictId;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalMarketStats",
                table: "ExternalMarketStats",
                column: "ExternalMarketStatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalMarketStats_Cities_CityId",
                table: "ExternalMarketStats",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalMarketStats_Districts_DistrictId",
                table: "ExternalMarketStats",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalMarketStats_Neighborhoods_NeighborhoodId",
                table: "ExternalMarketStats",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExternalMarketStats_ScopeLocation",
                table: "ExternalMarketStats",
                sql: "([Scope] = 1 AND [CityId] IS NULL AND [DistrictId] IS NULL AND [NeighborhoodId] IS NULL) OR ([Scope] = 2 AND [CityId] IS NOT NULL AND [DistrictId] IS NULL AND [NeighborhoodId] IS NULL) OR ([Scope] = 3 AND [CityId] IS NOT NULL AND [DistrictId] IS NOT NULL AND [NeighborhoodId] IS NULL) OR ([Scope] = 4 AND [CityId] IS NOT NULL AND [DistrictId] IS NOT NULL AND [NeighborhoodId] IS NOT NULL)");

            migrationBuilder.CreateTable(
                name: "MarketDataSyncLogs",
                columns: table => new
                {
                    MarketDataSyncLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    AffectedRowCount = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDataSyncLogs", x => x.MarketDataSyncLogId);
                });

            migrationBuilder.CreateTable(
                name: "NeighborhoodSeismicScenarioStats",
                columns: table => new
                {
                    NeighborhoodSeismicScenarioStatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NeighborhoodId = table.Column<int>(type: "int", nullable: false),
                    UavtCode = table.Column<int>(type: "int", nullable: false),
                    BuildingsBefore1980Count = table.Column<int>(type: "int", nullable: false),
                    Buildings1980To2000Count = table.Column<int>(type: "int", nullable: false),
                    BuildingsAfter2000Count = table.Column<int>(type: "int", nullable: false),
                    VeryHeavilyDamagedBuildingCount = table.Column<int>(type: "int", nullable: false),
                    HeavilyDamagedBuildingCount = table.Column<int>(type: "int", nullable: false),
                    ModeratelyDamagedBuildingCount = table.Column<int>(type: "int", nullable: false),
                    LightlyDamagedBuildingCount = table.Column<int>(type: "int", nullable: false),
                    EstimatedFatalityCount = table.Column<int>(type: "int", nullable: false),
                    TemporaryShelterNeedCount = table.Column<int>(type: "int", nullable: false),
                    DataSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeighborhoodSeismicScenarioStats", x => x.NeighborhoodSeismicScenarioStatId);
                    table.ForeignKey(
                        name: "FK_NeighborhoodSeismicScenarioStats_Neighborhoods_NeighborhoodId",
                        column: x => x.NeighborhoodId,
                        principalTable: "Neighborhoods",
                        principalColumn: "NeighborhoodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "BankLoanRates",
                keyColumn: "BankLoanRateId",
                keyValue: 1,
                columns: new[] { "DataSource", "ProviderKey", "SourceUrl" },
                values: new object[] { "FİBİ tahmini başlangıç verisi", "ziraat", null });

            migrationBuilder.UpdateData(
                table: "BankLoanRates",
                keyColumn: "BankLoanRateId",
                keyValue: 2,
                columns: new[] { "DataSource", "ProviderKey", "SourceUrl" },
                values: new object[] { "FİBİ tahmini başlangıç verisi", "halkbank", null });

            migrationBuilder.UpdateData(
                table: "BankLoanRates",
                keyColumn: "BankLoanRateId",
                keyValue: 3,
                columns: new[] { "DataSource", "ProviderKey", "SourceUrl" },
                values: new object[] { "FİBİ tahmini başlangıç verisi", "vakifbank", null });

            migrationBuilder.UpdateData(
                table: "BankLoanRates",
                keyColumn: "BankLoanRateId",
                keyValue: 4,
                columns: new[] { "DataSource", "ProviderKey", "SourceUrl" },
                values: new object[] { "FİBİ tahmini başlangıç verisi", "isbank", null });

            migrationBuilder.UpdateData(
                table: "BankLoanRates",
                keyColumn: "BankLoanRateId",
                keyValue: 5,
                columns: new[] { "DataSource", "ProviderKey", "SourceUrl" },
                values: new object[] { "FİBİ tahmini başlangıç verisi", "garanti", null });

            migrationBuilder.CreateIndex(
                name: "IX_BankLoanRates_ProviderKey",
                table: "BankLoanRates",
                column: "ProviderKey",
                unique: true,
                filter: "[ProviderKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMarketStats_CityId",
                table: "ExternalMarketStats",
                column: "CityId",
                unique: true,
                filter: "[Scope] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMarketStats_DistrictId",
                table: "ExternalMarketStats",
                column: "DistrictId",
                unique: true,
                filter: "[Scope] = 3");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMarketStats_NeighborhoodId",
                table: "ExternalMarketStats",
                column: "NeighborhoodId",
                unique: true,
                filter: "[Scope] = 4");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalMarketStats_Scope",
                table: "ExternalMarketStats",
                column: "Scope",
                unique: true,
                filter: "[Scope] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDataSyncLogs_ProviderKey_StartedAt",
                table: "MarketDataSyncLogs",
                columns: new[] { "ProviderKey", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NeighborhoodSeismicScenarioStats_NeighborhoodId",
                table: "NeighborhoodSeismicScenarioStats",
                column: "NeighborhoodId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ExternalMarketStats WHERE Scope <> 4;");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExternalMarketStats_ScopeLocation",
                table: "ExternalMarketStats");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalMarketStats_Cities_CityId",
                table: "ExternalMarketStats");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalMarketStats_Districts_DistrictId",
                table: "ExternalMarketStats");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalMarketStats_Neighborhoods_NeighborhoodId",
                table: "ExternalMarketStats");

            migrationBuilder.DropIndex(
                name: "IX_ExternalMarketStats_CityId",
                table: "ExternalMarketStats");

            migrationBuilder.DropIndex(
                name: "IX_ExternalMarketStats_DistrictId",
                table: "ExternalMarketStats");

            migrationBuilder.DropIndex(
                name: "IX_ExternalMarketStats_NeighborhoodId",
                table: "ExternalMarketStats");

            migrationBuilder.DropIndex(
                name: "IX_ExternalMarketStats_Scope",
                table: "ExternalMarketStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalMarketStats",
                table: "ExternalMarketStats");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "ExternalMarketStats");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ExternalMarketStats");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "ExternalMarketStats");

            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "ExternalMarketStats");

            migrationBuilder.DropColumn(
                name: "LastAppliedIndexPeriod",
                table: "ExternalMarketStats");

            migrationBuilder.AlterColumn<int>(
                name: "NeighborhoodId",
                table: "ExternalMarketStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SoilRiskClass",
                table: "ExternalMarketStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "ExternalMarketStatId",
                table: "ExternalMarketStats",
                newName: "ExternalNeighborhoodMarketStatId");

            migrationBuilder.RenameTable(
                name: "ExternalMarketStats",
                newName: "ExternalNeighborhoodMarketStats");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalNeighborhoodMarketStats",
                table: "ExternalNeighborhoodMarketStats",
                column: "ExternalNeighborhoodMarketStatId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalNeighborhoodMarketStats_NeighborhoodId",
                table: "ExternalNeighborhoodMarketStats",
                column: "NeighborhoodId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalNeighborhoodMarketStats_Neighborhoods_NeighborhoodId",
                table: "ExternalNeighborhoodMarketStats",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropTable(
                name: "MarketDataSyncLogs");

            migrationBuilder.DropTable(
                name: "NeighborhoodSeismicScenarioStats");

            migrationBuilder.DropIndex(
                name: "IX_BankLoanRates_ProviderKey",
                table: "BankLoanRates");

            migrationBuilder.DropColumn(
                name: "KvkkApprovedAt",
                table: "FibiServiceLeads");

            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "BankLoanRates");

            migrationBuilder.DropColumn(
                name: "ProviderKey",
                table: "BankLoanRates");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "BankLoanRates");
        }
    }
}
