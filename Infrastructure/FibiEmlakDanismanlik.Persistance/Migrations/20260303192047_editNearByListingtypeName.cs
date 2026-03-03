using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editNearByListingtypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListingType",
                table: "ListingNearbyPlaces",
                newName: "ListingTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingType_NearbyCategoryId_PlaceName",
                table: "ListingNearbyPlaces",
                newName: "IX_ListingNearbyPlaces_ListingId_ListingTypeId_NearbyCategoryId_PlaceName");

            migrationBuilder.RenameIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingType_IsActive",
                table: "ListingNearbyPlaces",
                newName: "IX_ListingNearbyPlaces_ListingId_ListingTypeId_IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListingTypeId",
                table: "ListingNearbyPlaces",
                newName: "ListingType");

            migrationBuilder.RenameIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingTypeId_NearbyCategoryId_PlaceName",
                table: "ListingNearbyPlaces",
                newName: "IX_ListingNearbyPlaces_ListingId_ListingType_NearbyCategoryId_PlaceName");

            migrationBuilder.RenameIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingTypeId_IsActive",
                table: "ListingNearbyPlaces",
                newName: "IX_ListingNearbyPlaces_ListingId_ListingType_IsActive");
        }
    }
}
