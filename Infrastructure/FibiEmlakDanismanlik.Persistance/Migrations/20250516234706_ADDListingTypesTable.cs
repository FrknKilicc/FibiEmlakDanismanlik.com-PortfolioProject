using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADDListingTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "rentalLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "rentalHousingListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "forSaleLandListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "listingTypes",
                columns: table => new
                {
                    ListingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listingTypes", x => x.ListingTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_ListingTypeId",
                table: "rentalLandListings",
                column: "ListingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_ListingTypeId",
                table: "rentalHousingListings",
                column: "ListingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalCommercialPropertyListings_ListingTypeId",
                table: "rentalCommercialPropertyListings",
                column: "ListingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_ListingTypeId",
                table: "forSaleLandListings",
                column: "ListingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_ListingTypeId",
                table: "forSaleHousingPropertyListings",
                column: "ListingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleCommercialPropertyListings_ListingTypeId",
                table: "forSaleCommercialPropertyListings",
                column: "ListingTypeId");

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
                name: "FK_rentalCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "rentalCommercialPropertyListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rentalHousingListings_listingTypes_ListingTypeId",
                table: "rentalHousingListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rentalLandListings_listingTypes_ListingTypeId",
                table: "rentalLandListings",
                column: "ListingTypeId",
                principalTable: "listingTypes",
                principalColumn: "ListingTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forSaleCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleHousingPropertyListings_listingTypes_ListingTypeId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_forSaleLandListings_listingTypes_ListingTypeId",
                table: "forSaleLandListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalCommercialPropertyListings_listingTypes_ListingTypeId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalHousingListings_listingTypes_ListingTypeId",
                table: "rentalHousingListings");

            migrationBuilder.DropForeignKey(
                name: "FK_rentalLandListings_listingTypes_ListingTypeId",
                table: "rentalLandListings");

            migrationBuilder.DropTable(
                name: "listingTypes");

            migrationBuilder.DropIndex(
                name: "IX_rentalLandListings_ListingTypeId",
                table: "rentalLandListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalHousingListings_ListingTypeId",
                table: "rentalHousingListings");

            migrationBuilder.DropIndex(
                name: "IX_rentalCommercialPropertyListings_ListingTypeId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleLandListings_ListingTypeId",
                table: "forSaleLandListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleHousingPropertyListings_ListingTypeId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_forSaleCommercialPropertyListings_ListingTypeId",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "ListingTypeId",
                table: "forSaleCommercialPropertyListings");
        }
    }
}
