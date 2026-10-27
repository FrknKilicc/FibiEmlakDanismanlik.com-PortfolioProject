using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editv3ForRentalAmenities : Migration
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
                    RentalCommercialPropertyListingRentalCommercialListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_RentalCommercialListingAmenities_AmenityDefinitionAmenityId",
                table: "RentalCommercialListingAmenities",
                column: "AmenityDefinitionAmenityId");

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
