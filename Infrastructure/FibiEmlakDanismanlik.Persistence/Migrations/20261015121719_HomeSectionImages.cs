using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HomeSectionImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeSectionImages",
                columns: table => new
                {
                    HomeSectionImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeSectionImages", x => x.HomeSectionImageId);
                });

            migrationBuilder.InsertData(
                table: "HomeSectionImages",
                columns: new[] { "HomeSectionImageId", "AltText", "ImageUrl", "IsActive", "SectionKey", "Slot" },
                values: new object[,]
                {
                    { 1, "Fibi Emlak Danışmanlık merkez ofis", "/Realshed/assets/images/resource/popular-place-1.jpg", true, "PopularPlaces", 1 },
                    { 2, "Fibi Emlak Danışmanlık ofis içi", "/Realshed/assets/images/resource/popular-place-2.jpg", true, "PopularPlaces", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeSectionImages_SectionKey_Slot",
                table: "HomeSectionImages",
                columns: new[] { "SectionKey", "Slot" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeSectionImages");
        }
    }
}
