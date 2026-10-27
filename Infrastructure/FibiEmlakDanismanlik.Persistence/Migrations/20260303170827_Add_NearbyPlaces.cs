using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_NearbyPlaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NearbyCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IconCss = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NearbyCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListingNearbyPlaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    ListingType = table.Column<int>(type: "int", nullable: false),
                    NearbyCategoryId = table.Column<int>(type: "int", nullable: false),
                    PlaceName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Stars = table.Column<byte>(type: "tinyint", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingNearbyPlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingNearbyPlaces_NearbyCategories_NearbyCategoryId",
                        column: x => x.NearbyCategoryId,
                        principalTable: "NearbyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingType_IsActive",
                table: "ListingNearbyPlaces",
                columns: new[] { "ListingId", "ListingType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ListingNearbyPlaces_ListingId_ListingType_NearbyCategoryId_PlaceName",
                table: "ListingNearbyPlaces",
                columns: new[] { "ListingId", "ListingType", "NearbyCategoryId", "PlaceName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingNearbyPlaces_NearbyCategoryId_IsActive",
                table: "ListingNearbyPlaces",
                columns: new[] { "NearbyCategoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_NearbyCategories_IsActive_SortOrder",
                table: "NearbyCategories",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_NearbyCategories_Key",
                table: "NearbyCategories",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingNearbyPlaces");

            migrationBuilder.DropTable(
                name: "NearbyCategories");
        }
    }
}
