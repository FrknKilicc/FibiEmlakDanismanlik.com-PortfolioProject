using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingNeighborhoodIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "rentalLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "rentalHousingListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "forSaleLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NeighborhoodId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_NeighborhoodId",
                table: "rentalLandListings",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_NeighborhoodId",
                table: "rentalHousingListings",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalCommercialPropertyListings_NeighborhoodId",
                table: "rentalCommercialPropertyListings",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_NeighborhoodId",
                table: "forSaleLandListings",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_NeighborhoodId",
                table: "forSaleHousingPropertyListings",
                column: "NeighborhoodId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleCommercialPropertyListings_NeighborhoodId",
                table: "forSaleCommercialPropertyListings",
                column: "NeighborhoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Neighborhoods_NeighborhoodId",
                table: "forSaleCommercialPropertyListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleHousingPropertyListings_Neighborhoods_NeighborhoodId",
                table: "forSaleHousingPropertyListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleLandListings_Neighborhoods_NeighborhoodId",
                table: "forSaleLandListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalCommercialPropertyListings_Neighborhoods_NeighborhoodId",
                table: "rentalCommercialPropertyListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalHousingListings_Neighborhoods_NeighborhoodId",
                table: "rentalHousingListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentalLandListings_Neighborhoods_NeighborhoodId",
                table: "rentalLandListings",
                column: "NeighborhoodId",
                principalTable: "Neighborhoods",
                principalColumn: "NeighborhoodId",
                onDelete: ReferentialAction.Restrict);

            // Mevcut ilanların NeighborhoodId'sini Neighborhood metninden doldur.
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
UPDATE l SET l.NeighborhoodId = (
    SELECT MIN(n.NeighborhoodId) FROM [Neighborhoods] n
    WHERE n.DistrictId = l.DistrictId
      AND ({Normalized("n.Name")} = {Normalized("l.Neighborhood")} + N' MAHALLESI' OR {Normalized("n.Name")} = {Normalized("l.Neighborhood")}))
FROM [{table}] l
WHERE l.NeighborhoodId IS NULL AND l.DistrictId IS NOT NULL AND l.Neighborhood IS NOT NULL;";

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forSaleCommercialPropertyListings_Neighborhoods_NeighborhoodId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleHousingPropertyListings_Neighborhoods_NeighborhoodId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleLandListings_Neighborhoods_NeighborhoodId",
                table: "forSaleLandListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalCommercialPropertyListings_Neighborhoods_NeighborhoodId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalHousingListings_Neighborhoods_NeighborhoodId",
                table: "rentalHousingListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalLandListings_Neighborhoods_NeighborhoodId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalLandListings_NeighborhoodId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalHousingListings_NeighborhoodId",
                table: "rentalHousingListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalCommercialPropertyListings_NeighborhoodId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleLandListings_NeighborhoodId",
                table: "forSaleLandListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleHousingPropertyListings_NeighborhoodId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleCommercialPropertyListings_NeighborhoodId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "NeighborhoodId",
                table: "forSaleCommercialPropertyListings");
        }
    }
}
