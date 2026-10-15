using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HomeTestimonialsSectionImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HomeSectionImages",
                columns: new[] { "HomeSectionImageId", "AltText", "ImageUrl", "IsActive", "SectionKey", "Slot" },
                values: new object[] { 3, "Yeni evinin önünde mutlu aile", "/Realshed/assets/images/background/testimonial-home.jpg", true, "Testimonials", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HomeSectionImages",
                keyColumn: "HomeSectionImageId",
                keyValue: 3);
        }
    }
}
