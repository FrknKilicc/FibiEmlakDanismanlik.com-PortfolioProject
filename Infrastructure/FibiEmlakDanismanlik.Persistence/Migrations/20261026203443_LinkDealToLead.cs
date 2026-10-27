using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkDealToLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinkedLeadId",
                table: "ListingDeals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LinkedLeadSource",
                table: "ListingDeals",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedLeadId",
                table: "ListingDeals");

            migrationBuilder.DropColumn(
                name: "LinkedLeadSource",
                table: "ListingDeals");
        }
    }
}
