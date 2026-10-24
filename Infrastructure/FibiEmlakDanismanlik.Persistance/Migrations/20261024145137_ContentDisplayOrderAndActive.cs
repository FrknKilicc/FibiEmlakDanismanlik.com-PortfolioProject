using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentDisplayOrderAndActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Testimonials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Testimonials",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "OurPartners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OurPartners",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Mevcut kayıtlar yayında kalır, sıra Id sırasıyla başlar
            migrationBuilder.Sql("UPDATE Testimonials SET DisplayOrder = Id; UPDATE OurPartners SET DisplayOrder = Id;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "OurPartners");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OurPartners");
        }
    }
}
