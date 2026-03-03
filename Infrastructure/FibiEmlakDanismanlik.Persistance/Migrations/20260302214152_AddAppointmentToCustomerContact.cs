using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentToCustomerContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PropertyListingTypeEnum",
                table: "customerContacts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerContactMail",
                table: "customerContacts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDateTime",
                table: "customerContacts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDateTime",
                table: "customerContacts");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyListingTypeEnum",
                table: "customerContacts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerContactMail",
                table: "customerContacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
