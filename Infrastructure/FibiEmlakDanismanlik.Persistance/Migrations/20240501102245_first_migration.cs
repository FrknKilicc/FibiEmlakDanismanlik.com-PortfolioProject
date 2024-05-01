using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class first_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    AboutUsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AboutUsTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutUsDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutUsImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.AboutUsId);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    AgentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AgentImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.AgentId);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMail1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMail2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber1 = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber2 = table.Column<int>(type: "int", nullable: false),
                    OfficeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "MainBanners",
                columns: table => new
                {
                    MainBannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainBannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainBannerDesc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainBanners", x => x.MainBannerId);
                });

            migrationBuilder.CreateTable(
                name: "MainCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainCategories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "frequentlyAskedQuestions",
                columns: table => new
                {
                    FrequentlyAskedQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frequentlyAskedQuestions", x => x.FrequentlyAskedQuestionId);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceDesc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "forSaleCommercialPropertyListings",
                columns: table => new
                {
                    ForSaleCommercialListingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TitleDeedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLoan = table.Column<bool>(type: "bit", nullable: false),
                    Exchange = table.Column<bool>(type: "bit", nullable: false),
                    Transferable = table.Column<bool>(type: "bit", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forSaleCommercialPropertyListings", x => x.ForSaleCommercialListingId);
                    table.ForeignKey(
                        name: "FK_forSaleCommercialPropertyListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rentalCommercialPropertyListings",
                columns: table => new
                {
                    RentalCommercialListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrossArea = table.Column<double>(type: "float", nullable: false),
                    NetArea = table.Column<double>(type: "float", nullable: false),
                    BuildingAge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: false),
                    Heating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Furnished = table.Column<bool>(type: "bit", nullable: false),
                    TitleDeedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dues = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentalCommercialPropertyListings", x => x.RentalCommercialListId);
                    table.ForeignKey(
                        name: "FK_rentalCommercialPropertyListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    BlogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TopNews = table.Column<bool>(type: "bit", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                    table.ForeignKey(
                        name: "FK_Blogs_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HousingCategories",
                columns: table => new
                {
                    HousingCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HousingCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousingCategories", x => x.HousingCategoryId);
                    table.ForeignKey(
                        name: "FK_HousingCategories_MainCategories_MainCategoryId",
                        column: x => x.MainCategoryId,
                        principalTable: "MainCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandCategories",
                columns: table => new
                {
                    LandCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LandCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandCategories", x => x.LandCategoryId);
                    table.ForeignKey(
                        name: "FK_LandCategories_MainCategories_MainCategoryId",
                        column: x => x.MainCategoryId,
                        principalTable: "MainCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "forSaleHousingPropertyListings",
                columns: table => new
                {
                    RentalHousingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsElevator = table.Column<bool>(type: "bit", nullable: false),
                    GrossArea = table.Column<double>(type: "float", nullable: false),
                    NetArea = table.Column<double>(type: "float", nullable: false),
                    OpenArea = table.Column<double>(type: "float", nullable: true),
                    BuildingAge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: false),
                    NumberOfBathRoom = table.Column<int>(type: "int", nullable: true),
                    NumberOfRoom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalNumberOfFloor = table.Column<int>(type: "int", nullable: false),
                    Heating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParkingLot = table.Column<bool>(type: "bit", nullable: true),
                    Furnished = table.Column<bool>(type: "bit", nullable: true),
                    UsageStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WithinTheComplex = table.Column<bool>(type: "bit", nullable: true),
                    Dues = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TitleDeedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeLoan = table.Column<bool>(type: "bit", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    HousingCategoryId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forSaleHousingPropertyListings", x => x.RentalHousingListId);
                    table.ForeignKey(
                        name: "FK_forSaleHousingPropertyListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_forSaleHousingPropertyListings_HousingCategories_HousingCategoryId",
                        column: x => x.HousingCategoryId,
                        principalTable: "HousingCategories",
                        principalColumn: "HousingCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rentalHousingListings",
                columns: table => new
                {
                    RentalHousingListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsElevator = table.Column<bool>(type: "bit", nullable: false),
                    GrossArea = table.Column<double>(type: "float", nullable: false),
                    NetArea = table.Column<double>(type: "float", nullable: false),
                    OpenArea = table.Column<double>(type: "float", nullable: true),
                    BuildingAge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: false),
                    NumberOfBathRoom = table.Column<int>(type: "int", nullable: true),
                    NumberOfRoom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalNumberOfFloor = table.Column<int>(type: "int", nullable: false),
                    Heating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParkingLot = table.Column<bool>(type: "bit", nullable: true),
                    Furnished = table.Column<bool>(type: "bit", nullable: true),
                    WithinTheComplex = table.Column<bool>(type: "bit", nullable: true),
                    Dues = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    HousingCategoryId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentalHousingListings", x => x.RentalHousingListId);
                    table.ForeignKey(
                        name: "FK_rentalHousingListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rentalHousingListings_HousingCategories_HousingCategoryId",
                        column: x => x.HousingCategoryId,
                        principalTable: "HousingCategories",
                        principalColumn: "HousingCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "forSaleLandListings",
                columns: table => new
                {
                    ForSaleLandListingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZoningStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerSquareMeter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParcelNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MapSheetNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorAreaRatio = table.Column<double>(type: "float", nullable: false),
                    BaseAreaRatio = table.Column<double>(type: "float", nullable: false),
                    ZoningPlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleDeedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevelopmentRight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLoan = table.Column<bool>(type: "bit", nullable: false),
                    Exchange = table.Column<bool>(type: "bit", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    LandCategoryId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forSaleLandListings", x => x.ForSaleLandListingId);
                    table.ForeignKey(
                        name: "FK_forSaleLandListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_forSaleLandListings_LandCategories_LandCategoryId",
                        column: x => x.LandCategoryId,
                        principalTable: "LandCategories",
                        principalColumn: "LandCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rentalLandListings",
                columns: table => new
                {
                    RentalLandListingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyNo = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZoningStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    Rent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerSquareMeter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParcelNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MapSheetNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorAreaRatio = table.Column<double>(type: "float", nullable: false),
                    BaseAreaRatio = table.Column<double>(type: "float", nullable: false),
                    ZoningPlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleDeedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DevelopmentRight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BestDeals = table.Column<bool>(type: "bit", nullable: true),
                    LandCategoryId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    PropImgUrl1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropImgUrl15 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentalLandListings", x => x.RentalLandListingId);
                    table.ForeignKey(
                        name: "FK_rentalLandListings_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rentalLandListings_LandCategories_LandCategoryId",
                        column: x => x.LandCategoryId,
                        principalTable: "LandCategories",
                        principalColumn: "LandCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_AuthorId",
                table: "Blogs",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_HousingCategories_MainCategoryId",
                table: "HousingCategories",
                column: "MainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LandCategories_MainCategoryId",
                table: "LandCategories",
                column: "MainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleCommercialPropertyListings_AgentId",
                table: "forSaleCommercialPropertyListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_AgentId",
                table: "forSaleHousingPropertyListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleHousingPropertyListings_HousingCategoryId",
                table: "forSaleHousingPropertyListings",
                column: "HousingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_AgentId",
                table: "forSaleLandListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_forSaleLandListings_LandCategoryId",
                table: "forSaleLandListings",
                column: "LandCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalCommercialPropertyListings_AgentId",
                table: "rentalCommercialPropertyListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_AgentId",
                table: "rentalHousingListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalHousingListings_HousingCategoryId",
                table: "rentalHousingListings",
                column: "HousingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_AgentId",
                table: "rentalLandListings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_rentalLandListings_LandCategoryId",
                table: "rentalLandListings",
                column: "LandCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUs");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "MainBanners");

            migrationBuilder.DropTable(
                name: "forSaleCommercialPropertyListings");

            migrationBuilder.DropTable(
                name: "forSaleHousingPropertyListings");

            migrationBuilder.DropTable(
                name: "forSaleLandListings");

            migrationBuilder.DropTable(
                name: "frequentlyAskedQuestions");

            migrationBuilder.DropTable(
                name: "rentalCommercialPropertyListings");

            migrationBuilder.DropTable(
                name: "rentalHousingListings");

            migrationBuilder.DropTable(
                name: "rentalLandListings");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "HousingCategories");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "LandCategories");

            migrationBuilder.DropTable(
                name: "MainCategories");
        }
    }
}
