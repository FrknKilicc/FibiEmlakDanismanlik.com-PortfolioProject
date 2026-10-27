using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    public partial class RentalPropertyAmenities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) ForSale ListingType FK'leri kaldır (bunlar mevcutsa sorun yok, ama yine de güvenli yapalım)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId')
    ALTER TABLE [dbo].[forSaleCommercialPropertyListings] DROP CONSTRAINT [FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId')
    ALTER TABLE [dbo].[forSaleHousingPropertyListings] DROP CONSTRAINT [FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_forSaleLandListings_listingTypes_ListingTypeId')
    ALTER TABLE [dbo].[forSaleLandListings] DROP CONSTRAINT [FK_forSaleLandListings_listingTypes_ListingTypeId];
");

            // 2) MainCategories'e bağlı FK'leri güvenli kaldır
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_HousingCategories_MainCategories_MainCategoryId')
    ALTER TABLE [dbo].[HousingCategories] DROP CONSTRAINT [FK_HousingCategories_MainCategories_MainCategoryId];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LandCategories_MainCategories_MainCategoryId')
    ALTER TABLE [dbo].[LandCategories] DROP CONSTRAINT [FK_LandCategories_MainCategories_MainCategoryId];
");

            // 3) Index'leri güvenli kaldır
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_LandCategories_MainCategoryId'
      AND object_id = OBJECT_ID(N'dbo.LandCategories')
)
    DROP INDEX [IX_LandCategories_MainCategoryId] ON [dbo].[LandCategories];

IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_HousingCategories_MainCategoryId'
      AND object_id = OBJECT_ID(N'dbo.HousingCategories')
)
    DROP INDEX [IX_HousingCategories_MainCategoryId] ON [dbo].[HousingCategories];
");

            // 4) Kolonları güvenli kaldır
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.LandCategories', 'MainCategoryId') IS NOT NULL
    ALTER TABLE [dbo].[LandCategories] DROP COLUMN [MainCategoryId];

IF COL_LENGTH('dbo.HousingCategories', 'MainCategoryId') IS NOT NULL
    ALTER TABLE [dbo].[HousingCategories] DROP COLUMN [MainCategoryId];
");

            // 5) MainCategories tablosunu en sona bırakıp güvenli drop et
            // (FK + index + column işleri bittikten sonra)
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.MainCategories', N'U') IS NOT NULL
    DROP TABLE [dbo].[MainCategories];
");

            // 6) ListingTypeId kolonlarını non-null yap
            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleLandListings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 7) Rental Amenities tabloları
            migrationBuilder.CreateTable(
                name: "RentalCommercialListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalCommercialListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalCommercialPropertyListingRentalCommercialListId = table.Column<int>(type: "int", nullable: false),
                    AmenityDefinitionAmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalCommercialListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalCommercialListingAmenities_AmenityDefinitions_AmenityDefinitionAmenityId",
                        column: x => x.AmenityDefinitionAmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalCommercialListingAmenities_rentalCommercialPropertyListings_RentalCommercialPropertyListingRentalCommercialListId",
                        column: x => x.RentalCommercialPropertyListingRentalCommercialListId,
                        principalTable: "rentalCommercialPropertyListings",
                        principalColumn: "RentalCommercialListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalHousingListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalHousingListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalHousingListingRentalHousingListId = table.Column<int>(type: "int", nullable: false),
                    AmenityDefinitionAmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalHousingListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalHousingListingAmenities_AmenityDefinitions_AmenityDefinitionAmenityId",
                        column: x => x.AmenityDefinitionAmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalHousingListingAmenities_rentalHousingListings_RentalHousingListingRentalHousingListId",
                        column: x => x.RentalHousingListingRentalHousingListId,
                        principalTable: "rentalHousingListings",
                        principalColumn: "RentalHousingListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalLandListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalLandListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalLandListingId = table.Column<int>(type: "int", nullable: false),
                    AmenityDefinitionAmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLandListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalLandListingAmenities_AmenityDefinitions_AmenityDefinitionAmenityId",
                        column: x => x.AmenityDefinitionAmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalLandListingAmenities_rentalLandListings_RentalLandListingId",
                        column: x => x.RentalLandListingId,
                        principalTable: "rentalLandListings",
                        principalColumn: "RentalLandListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_AmenityDefinitionAmenityId",
                table: "RentalCommercialListingAmenities",
                column: "AmenityDefinitionAmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_RentalCommercialPropertyListingRentalCommercialListId",
                table: "RentalCommercialListingAmenities",
                column: "RentalCommercialPropertyListingRentalCommercialListId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalHousingListingAmenities_AmenityDefinitionAmenityId",
                table: "RentalHousingListingAmenities",
                column: "AmenityDefinitionAmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalHousingListingAmenities_RentalHousingListingRentalHousingListId",
                table: "RentalHousingListingAmenities",
                column: "RentalHousingListingRentalHousingListId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalLandListingAmenities_AmenityDefinitionAmenityId",
                table: "RentalLandListingAmenities",
                column: "AmenityDefinitionAmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalLandListingAmenities_RentalLandListingId",
                table: "RentalLandListingAmenities",
                column: "RentalLandListingId");

            // 8) ListingType FK'lerini tekrar ekle (varsa önce silmiştik)
            migrationBuilder.AddForeignKey(
                name: "FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleHousingPropertyListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleLandListings_listingTypes_ListingTypeId",
                table: "forSaleLandListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down tarafını şimdilik orijinal bırakabilirsin.
            // Localde sorunu çözen esas kısım Up. Prod'a gidecekse Down'ı da idempotent yapmak gerekir.

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleLandListings_listingTypes_ListingTypeId",
                table: "forSaleLandListings");

            migrationBuilder.DropTable(name: "RentalCommercialListingAmenities");
            migrationBuilder.DropTable(name: "RentalHousingListingAmenities");
            migrationBuilder.DropTable(name: "RentalLandListingAmenities");

            migrationBuilder.AddColumn<int>(
                name: "MainCategoryId",
                table: "LandCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MainCategoryId",
                table: "HousingCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleLandListings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "MainCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_MainCategories", x => x.CategoryId); });

            migrationBuilder.CreateIndex(
                name: "IX_LandCategories_MainCategoryId",
                table: "LandCategories",
                column: "MainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HousingCategories_MainCategoryId",
                table: "HousingCategories",
                column: "MainCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleHousingPropertyListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_forSaleLandListings_listingTypes_ListingTypeId",
                table: "forSaleLandListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HousingCategories_MainCategories_MainCategoryId",
                table: "HousingCategories",
                column: "MainCategoryId",
                principalTable: "MainCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandCategories_MainCategories_MainCategoryId",
                table: "LandCategories",
                column: "MainCategoryId",
                principalTable: "MainCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}