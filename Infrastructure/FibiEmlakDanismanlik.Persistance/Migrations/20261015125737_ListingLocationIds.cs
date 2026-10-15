using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingLocationIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "rentalLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "rentalLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "rentalHousingListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "rentalHousingListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "forSaleLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "forSaleLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_CityId",
                table: "rentalLandListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_DistrictId",
                table: "rentalLandListings",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_CityId",
                table: "rentalHousingListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_DistrictId",
                table: "rentalHousingListings",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalCommercialPropertyListings_CityId",
                table: "rentalCommercialPropertyListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalCommercialPropertyListings_DistrictId",
                table: "rentalCommercialPropertyListings",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_CityId",
                table: "forSaleLandListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_DistrictId",
                table: "forSaleLandListings",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_CityId",
                table: "forSaleHousingPropertyListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_DistrictId",
                table: "forSaleHousingPropertyListings",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleCommercialPropertyListings_CityId",
                table: "forSaleCommercialPropertyListings",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleCommercialPropertyListings_DistrictId",
                table: "forSaleCommercialPropertyListings",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Cities_CityId",
                table: "forSaleCommercialPropertyListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Districts_DistrictId",
                table: "forSaleCommercialPropertyListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleHousingPropertyListings_Cities_CityId",
                table: "forSaleHousingPropertyListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleHousingPropertyListings_Districts_DistrictId",
                table: "forSaleHousingPropertyListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleLandListings_Cities_CityId",
                table: "forSaleLandListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleLandListings_Districts_DistrictId",
                table: "forSaleLandListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalCommercialPropertyListings_Cities_CityId",
                table: "rentalCommercialPropertyListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalCommercialPropertyListings_Districts_DistrictId",
                table: "rentalCommercialPropertyListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalHousingListings_Cities_CityId",
                table: "rentalHousingListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalHousingListings_Districts_DistrictId",
                table: "rentalHousingListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalLandListings_Cities_CityId",
                table: "rentalLandListings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalLandListings_Districts_DistrictId",
                table: "rentalLandListings",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.Restrict);

            // Mevcut ilanların CityId/DistrictId'sini City/District metninden doldur.
            // Yeni kayıtlarda bunu DbContext.SaveChanges yapıyor (LocationNameNormalizer).
            foreach (var table in ListingTables)
            {
                migrationBuilder.Sql(BuildBackfillSql(table));
            }
        }

        private static readonly string[] ListingTables =
        {
            "forSaleHousingPropertyListings",
            "forSaleLandListings",
            "forSaleCommercialPropertyListings",
            "rentalHousingListings",
            "rentalLandListings",
            "rentalCommercialPropertyListings"
        };

        // LocationNameNormalizer'ın SQL karşılığı: trim, tr büyük harf, Türkçe harfleri ASCII'ye indir
        private static string Normalized(string column) =>
            $"REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(" +
            $"UPPER(LTRIM(RTRIM({column})) COLLATE Turkish_CS_AS)," +
            $"N'İ',N'I'),N'Ş',N'S'),N'Ğ',N'G'),N'Ü',N'U'),N'Ö',N'O'),N'Ç',N'C'),N'Â',N'A'),N'Î',N'I'),N'Û',N'U')";

        private static string BuildBackfillSql(string table) => $@"
UPDATE l SET l.CityId = c.CityId
FROM [{table}] l
JOIN [Cities] c ON {Normalized("c.Name")} = {Normalized("l.City")}
WHERE l.CityId IS NULL AND l.City IS NOT NULL;

UPDATE l SET l.DistrictId = d.DistrictId
FROM [{table}] l
JOIN [Districts] d ON d.CityId = l.CityId AND {Normalized("d.Name")} = {Normalized("l.District")}
WHERE l.DistrictId IS NULL AND l.CityId IS NOT NULL AND l.District IS NOT NULL;";

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Cities_CityId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Districts_DistrictId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleHousingPropertyListings_Cities_CityId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleHousingPropertyListings_Districts_DistrictId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleLandListings_Cities_CityId",
                table: "forSaleLandListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleLandListings_Districts_DistrictId",
                table: "forSaleLandListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalCommercialPropertyListings_Cities_CityId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalCommercialPropertyListings_Districts_DistrictId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalHousingListings_Cities_CityId",
                table: "rentalHousingListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalHousingListings_Districts_DistrictId",
                table: "rentalHousingListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalLandListings_Cities_CityId",
                table: "rentalLandListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalLandListings_Districts_DistrictId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalLandListings_CityId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalLandListings_DistrictId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalHousingListings_CityId",
                table: "rentalHousingListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalHousingListings_DistrictId",
                table: "rentalHousingListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalCommercialPropertyListings_CityId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalCommercialPropertyListings_DistrictId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleLandListings_CityId",
                table: "forSaleLandListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleLandListings_DistrictId",
                table: "forSaleLandListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleHousingPropertyListings_CityId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleHousingPropertyListings_DistrictId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleCommercialPropertyListings_CityId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleCommercialPropertyListings_DistrictId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "forSaleCommercialPropertyListings");
        }
    }
}
