using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingIdRangesAndLandCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // İlan detay sayfası yalnızca Id ile arama yaptığı için konut, arsa ve ticari tabloları arasında Id çakışmasını önlemek amacıyla identity aralıkları ayrılmıştır.
            migrationBuilder.Sql("IF IDENT_CURRENT('forSaleHousingPropertyListings') < 1000000 DBCC CHECKIDENT ('forSaleHousingPropertyListings', RESEED, 1000000);");
            migrationBuilder.Sql("IF IDENT_CURRENT('forSaleLandListings') < 2000000 DBCC CHECKIDENT ('forSaleLandListings', RESEED, 2000000);");
            migrationBuilder.Sql("IF IDENT_CURRENT('forSaleCommercialPropertyListings') < 3000000 DBCC CHECKIDENT ('forSaleCommercialPropertyListings', RESEED, 3000000);");
            migrationBuilder.Sql("IF IDENT_CURRENT('rentalHousingListings') < 1000000 DBCC CHECKIDENT ('rentalHousingListings', RESEED, 1000000);");
            migrationBuilder.Sql("IF IDENT_CURRENT('rentalLandListings') < 2000000 DBCC CHECKIDENT ('rentalLandListings', RESEED, 2000000);");
            migrationBuilder.Sql("IF IDENT_CURRENT('rentalCommercialPropertyListings') < 3000000 DBCC CHECKIDENT ('rentalCommercialPropertyListings', RESEED, 3000000);");

            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Konut İmarlı') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Konut İmarlı');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Ticari İmarlı') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Ticari İmarlı');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Konut + Ticari İmarlı') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Konut + Ticari İmarlı');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Sanayi İmarlı') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Sanayi İmarlı');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Turizm İmarlı') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Turizm İmarlı');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Tarla') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Tarla');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Bağ & Bahçe') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Bağ & Bahçe');");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM LandCategories WHERE LandCategoryName = N'Zeytinlik') INSERT INTO LandCategories (LandCategoryName) VALUES (N'Zeytinlik');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM LandCategories WHERE LandCategoryName IN (N'Konut İmarlı', N'Ticari İmarlı', N'Konut + Ticari İmarlı', N'Sanayi İmarlı', N'Turizm İmarlı', N'Tarla', N'Bağ & Bahçe', N'Zeytinlik') AND LandCategoryId NOT IN (SELECT LandCategoryId FROM forSaleLandListings UNION SELECT LandCategoryId FROM rentalLandListings);");

            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(ForSaleHousingListId) FROM forSaleHousingPropertyListings), 0); DBCC CHECKIDENT ('forSaleHousingPropertyListings', RESEED, @m);");
            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(ForSaleLandListingId) FROM forSaleLandListings), 0); DBCC CHECKIDENT ('forSaleLandListings', RESEED, @m);");
            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(ForSaleCommercialListingId) FROM forSaleCommercialPropertyListings), 0); DBCC CHECKIDENT ('forSaleCommercialPropertyListings', RESEED, @m);");
            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(RentalHousingListId) FROM rentalHousingListings), 0); DBCC CHECKIDENT ('rentalHousingListings', RESEED, @m);");
            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(RentalLandListingId) FROM rentalLandListings), 0); DBCC CHECKIDENT ('rentalLandListings', RESEED, @m);");
            migrationBuilder.Sql("DECLARE @m int = ISNULL((SELECT MAX(RentalCommercialListId) FROM rentalCommercialPropertyListings), 0); DBCC CHECKIDENT ('rentalCommercialPropertyListings', RESEED, @m);");
        }
    }
}
