using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editv12ForRentalAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentalHousingListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalHousingListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalHousingListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalHousingListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalHousingListingAmenities_rentalHousingListings_RentalHousingListId",
                        column: x => x.RentalHousingListId,
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
                    RentalLandListingId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLandListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalLandListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
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
                name: "IX_RentalHousingListingAmenities_AmenityId",
                table: "RentalHousingListingAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalHousingListingAmenities_RentalHousingListId",
                table: "RentalHousingListingAmenities",
                column: "RentalHousingListId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalLandListingAmenities_AmenityId",
                table: "RentalLandListingAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalLandListingAmenities_RentalLandListingId",
                table: "RentalLandListingAmenities",
                column: "RentalLandListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalHousingListingAmenities");

            migrationBuilder.DropTable(
                name: "RentalLandListingAmenities");
        }
    }
}
