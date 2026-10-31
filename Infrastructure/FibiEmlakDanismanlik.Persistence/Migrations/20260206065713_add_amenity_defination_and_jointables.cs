using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_amenity_defination_and_jointables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmenityDefinitions",
                columns: table => new
                {
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    AppliesToHousing = table.Column<bool>(type: "bit", nullable: false),
                    AppliesToLand = table.Column<bool>(type: "bit", nullable: false),
                    AppliesToCommercial = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityDefinitions", x => x.AmenityId);
                });

            migrationBuilder.CreateTable(
                name: "ForSaleCommercialListingAmenities",
                columns: table => new
                {
                    ForSaleCommercialListingId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForSaleCommercialListingAmenities", x => new { x.ForSaleCommercialListingId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_ForSaleCommercialListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForSaleCommercialListingAmenities_forSaleCommercialPropertyListings_ForSaleCommercialListingId",
                        column: x => x.ForSaleCommercialListingId,
                        principalTable: "forSaleCommercialPropertyListings",
                        principalColumn: "ForSaleCommercialListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForSaleHousingListingAmenities",
                columns: table => new
                {
                    ForSaleHousingListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForSaleHousingListingAmenities", x => new { x.ForSaleHousingListId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_ForSaleHousingListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForSaleHousingListingAmenities_forSaleHousingPropertyListings_ForSaleHousingListId",
                        column: x => x.ForSaleHousingListId,
                        principalTable: "forSaleHousingPropertyListings",
                        principalColumn: "ForSaleHousingListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForSaleLandListingAmenities",
                columns: table => new
                {
                    ForSaleLandListId = table.Column<int>(type: "int", nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForSaleLandListingAmenities", x => new { x.ForSaleLandListId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_ForSaleLandListingAmenities_AmenityDefinitions_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "AmenityDefinitions",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForSaleLandListingAmenities_forSaleLandListings_ForSaleLandListId",
                        column: x => x.ForSaleLandListId,
                        principalTable: "forSaleLandListings",
                        principalColumn: "ForSaleLandListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmenityDefinitions_Name",
                table: "AmenityDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ForSaleCommercialListingAmenities_AmenityId_ForSaleCommercialListingId",
                table: "ForSaleCommercialListingAmenities",
                columns: new[] { "AmenityId", "ForSaleCommercialListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_ForSaleHousingListingAmenities_AmenityId_ForSaleHousingListId",
                table: "ForSaleHousingListingAmenities",
                columns: new[] { "AmenityId", "ForSaleHousingListId" });

            migrationBuilder.CreateIndex(
                name: "IX_ForSaleLandListingAmenities_AmenityId_ForSaleLandListId",
                table: "ForSaleLandListingAmenities",
                columns: new[] { "AmenityId", "ForSaleLandListId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForSaleCommercialListingAmenities");

            migrationBuilder.DropTable(
                name: "ForSaleHousingListingAmenities");

            migrationBuilder.DropTable(
                name: "ForSaleLandListingAmenities");

            migrationBuilder.DropTable(
                name: "AmenityDefinitions");
        }
    }
}
