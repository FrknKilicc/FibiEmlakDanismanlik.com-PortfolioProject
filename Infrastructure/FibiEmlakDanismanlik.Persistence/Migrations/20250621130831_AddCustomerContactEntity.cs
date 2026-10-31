using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerContactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customerContacts",
                columns: table => new
                {
                    CustomerContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerContactMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerContactMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PropertyListingId = table.Column<int>(type: "int", nullable: true),
                    PropertyListingTypeEnum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerContacts", x => x.CustomerContactId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customerContacts");
        }
    }
}
