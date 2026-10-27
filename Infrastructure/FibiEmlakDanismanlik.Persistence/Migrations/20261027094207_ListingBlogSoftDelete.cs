using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingBlogSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "rentalLandListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "rentalHousingListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "rentalCommercialPropertyListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "forSaleLandListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "forSaleHousingPropertyListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "forSaleCommercialPropertyListings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Blogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Blogs");
        }
    }
}
