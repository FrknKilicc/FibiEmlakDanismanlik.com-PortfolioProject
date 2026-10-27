using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class doPropertyNo_Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalLandListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalHousingListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleLandListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: true,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalLandListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalHousingListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalHousingListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "rentalCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_RentalCommercialPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleLandListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleLandListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleHousingPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleHousingPropertyListings_Table");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyNo",
                table: "forSaleCommercialPropertyListings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValueSql: "NEXT VALUE FOR Seq_ForSaleCommercialPropertyListings_Table");
        }
    }
}
