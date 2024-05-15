using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Listing_Props_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dues",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "Furnished",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.RenameColumn(
                name: "RentalHousingListId",
                table: "forSaleHousingPropertyListings",
                newName: "ForSaleHousingListId");

            migrationBuilder.AlterColumn<string>(
                name: "ZoningPlan",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TitleDeedStatus",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParcelNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MapSheetNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "FloorAreaRatio",
                table: "rentalLandListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "DevelopmentRight",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "BaseAreaRatio",
                table: "rentalLandListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "rentalLandListings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BlackBox",
                table: "rentalHousingListings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facade",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBalconies",
                table: "rentalHousingListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "rentalHousingListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TitleDeedStatus",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "NetArea",
                table: "rentalCommercialPropertyListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "GrossArea",
                table: "rentalCommercialPropertyListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "AddressDesc",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Facade",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBathrooms",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfKitchens",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSection",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "frequentlyAskedQuestions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ZoningPlan",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParcelNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MapSheetNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "LandLoan",
                table: "forSaleLandListings",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<double>(
                name: "FloorAreaRatio",
                table: "forSaleLandListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "Exchange",
                table: "forSaleLandListings",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "DevelopmentRight",
                table: "forSaleLandListings",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "BaseAreaRatio",
                table: "forSaleLandListings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "forSaleLandListings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SharePercentage",
                table: "forSaleLandListings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BlackBox",
                table: "forSaleHousingPropertyListings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Exchange",
                table: "forSaleHousingPropertyListings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Facade",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBalconies",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "forSaleHousingPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facade",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GrossArea",
                table: "forSaleCommercialPropertyListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBathrooms",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfFloors",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfKitchens",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSection",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl16",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl17",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl18",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl19",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl20",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl21",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl22",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl23",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl24",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl25",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl26",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl27",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl28",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl29",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropImgUrl30",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SharePercentage",
                table: "forSaleCommercialPropertyListings",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "rentalLandListings");

            migrationBuilder.DropColumn(
                name: "BlackBox",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "Facade",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "NumberOfBalconies",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "rentalHousingListings");

            migrationBuilder.DropColumn(
                name: "Facade",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfBathrooms",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfKitchens",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfSection",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "rentalCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "SharePercentage",
                table: "forSaleLandListings");

            migrationBuilder.DropColumn(
                name: "BlackBox",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "Facade",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfBalconies",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "forSaleHousingPropertyListings");

            migrationBuilder.DropColumn(
                name: "Facade",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "GrossArea",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfBathrooms",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfFloors",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfKitchens",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "NumberOfSection",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl16",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl17",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl18",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl19",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl20",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl21",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl22",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl23",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl24",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl25",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl26",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl27",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl28",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl29",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "PropImgUrl30",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.DropColumn(
                name: "SharePercentage",
                table: "forSaleCommercialPropertyListings");

            migrationBuilder.RenameColumn(
                name: "ForSaleHousingListId",
                table: "forSaleHousingPropertyListings",
                newName: "RentalHousingListId");

            migrationBuilder.AlterColumn<string>(
                name: "ZoningPlan",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TitleDeedStatus",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParcelNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MapSheetNumber",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FloorAreaRatio",
                table: "rentalLandListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DevelopmentRight",
                table: "rentalLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "BaseAreaRatio",
                table: "rentalLandListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TitleDeedStatus",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "NetArea",
                table: "rentalCommercialPropertyListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "GrossArea",
                table: "rentalCommercialPropertyListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AddressDesc",
                table: "rentalCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Dues",
                table: "rentalCommercialPropertyListings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Furnished",
                table: "rentalCommercialPropertyListings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "frequentlyAskedQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ZoningPlan",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParcelNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MapSheetNumber",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LandLoan",
                table: "forSaleLandListings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FloorAreaRatio",
                table: "forSaleLandListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Exchange",
                table: "forSaleLandListings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DevelopmentRight",
                table: "forSaleLandListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "BaseAreaRatio",
                table: "forSaleLandListings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PropImgUrl1",
                table: "forSaleCommercialPropertyListings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
