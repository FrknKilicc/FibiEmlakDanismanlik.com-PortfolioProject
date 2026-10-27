using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSequencess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "Seq_ForSaleCommercialPropertyListings_Table",
                startValue: 500000L);

            migrationBuilder.CreateSequence<int>(
                name: "Seq_ForSaleHousingPropertyListings_Table");

            migrationBuilder.CreateSequence<int>(
                name: "Seq_ForSaleLandListings_Table",
                startValue: 200000L);

            migrationBuilder.CreateSequence<int>(
                name: "Seq_RentalCommercialPropertyListings_Table",
                startValue: 1000000L);

            migrationBuilder.CreateSequence<int>(
                name: "Seq_RentalHousingListings_Table",
                startValue: 100000L);

            migrationBuilder.CreateSequence<int>(
                name: "Seq_RentalLandListings_Table",
                startValue: 300000L);

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalLandListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalHousingListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleLandListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "Seq_ForSaleCommercialPropertyListings_Table");

            migrationBuilder.DropSequence(
                name: "Seq_ForSaleHousingPropertyListings_Table");

            migrationBuilder.DropSequence(
                name: "Seq_ForSaleLandListings_Table");

            migrationBuilder.DropSequence(
                name: "Seq_RentalCommercialPropertyListings_Table");

            migrationBuilder.DropSequence(
                name: "Seq_RentalHousingListings_Table");

            migrationBuilder.DropSequence(
                name: "Seq_RentalLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalLandListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalHousingListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleLandListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table");
        }
    }
}
