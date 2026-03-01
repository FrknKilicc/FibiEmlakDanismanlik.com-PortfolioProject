using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editv10ForRentalAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalHousingListingAmenities");

            migrationBuilder.DropTable(
                name: "RentalLandListingAmenities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentalHousingListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmenityDefinitionAmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalHousingListingRentalHousingListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalHousingListId = table.Column<int>(type: "int", nullable: false)
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
                    AmenityDefinitionAmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalLandListingId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalLandListId = table.Column<int>(type: "int", nullable: false)
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
        }
    }
}
