using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editv7Fixingforrentalamenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentalCommercialListingAmenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalCommercialListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
                    RentalCommercialPropertyListingRentalCommercialListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalCommercialListingAmenities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalCommercialListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalCommercialListingAmenities_rentalCommercialPropertyListings_RentalCommercialListId",
                        column: x => x.RentalCommercialListId,
                        principalTable: "rentalCommercialPropertyListings",
                        principalColumn: "RentalCommercialListId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalCommercialListingAmenities_rentalCommercialPropertyListings_RentalCommercialPropertyListingRentalCommercialListId",
                        column: x => x.RentalCommercialPropertyListingRentalCommercialListId,
                        principalTable: "rentalCommercialPropertyListings",
                        principalColumn: "RentalCommercialListId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_AmenityId",
                table: "RentalCommercialListingAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_RentalCommercialListId",
                table: "RentalCommercialListingAmenities",
                column: "RentalCommercialListId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_RentalCommercialPropertyListingRentalCommercialListId",
                table: "RentalCommercialListingAmenities",
                column: "RentalCommercialPropertyListingRentalCommercialListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalCommercialListingAmenities");
        }
    }
}
