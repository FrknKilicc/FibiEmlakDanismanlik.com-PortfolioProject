using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListingImages",
                columns: table => new
                {
                    ListingImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingTypeId = table.Column<int>(type: "int", nullable: false),
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    MediumUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ThumbUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsCover = table.Column<bool>(type: "bit", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    StoragePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingImages", x => x.ListingImageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListingImages_ListingTypeId_ListingId_SortOrder",
                table: "ListingImages",
                columns: new[] { "ListingTypeId", "ListingId", "SortOrder" });

            // Mevcut PropImgUrl1..30 değerleri ListingImages'a aktarılır; eski kolonlar silinmez, okuyucular bozulmaz
            migrationBuilder.Sql(@"
-- 1. forSaleLandListings -> ListingTypeId = 1
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    1 AS [ListingTypeId],
    t.[ForSaleLandListingId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[ForSaleLandListingId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[ForSaleLandListingId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [forSaleLandListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 1 AND img.[ListingId] = t.[ForSaleLandListingId]
  );

-- 2. forSaleHousingPropertyListings -> ListingTypeId = 2
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    2 AS [ListingTypeId],
    t.[ForSaleHousingListId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[ForSaleHousingListId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[ForSaleHousingListId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [forSaleHousingPropertyListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 2 AND img.[ListingId] = t.[ForSaleHousingListId]
  );

-- 3. forSaleCommercialPropertyListings -> ListingTypeId = 3
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    3 AS [ListingTypeId],
    t.[ForSaleCommercialListingId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[ForSaleCommercialListingId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[ForSaleCommercialListingId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [forSaleCommercialPropertyListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 3 AND img.[ListingId] = t.[ForSaleCommercialListingId]
  );

-- 4. rentalLandListings -> ListingTypeId = 7
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    7 AS [ListingTypeId],
    t.[RentalLandListingId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[RentalLandListingId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[RentalLandListingId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [rentalLandListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 7 AND img.[ListingId] = t.[RentalLandListingId]
  );

-- 5. rentalHousingListings -> ListingTypeId = 8
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    8 AS [ListingTypeId],
    t.[RentalHousingListId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[RentalHousingListId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[RentalHousingListId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [rentalHousingListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 8 AND img.[ListingId] = t.[RentalHousingListId]
  );

-- 6. rentalCommercialPropertyListings -> ListingTypeId = 9
INSERT INTO [ListingImages] ([ListingTypeId], [ListingId], [Url], [MediumUrl], [ThumbUrl], [SortOrder], [IsCover], [AltText], [Width], [Height], [StoragePath], [CreatedAtUtc])
SELECT
    9 AS [ListingTypeId],
    t.[RentalCommercialListId] AS [ListingId],
    LTRIM(RTRIM(v.Url)) AS [Url],
    NULL AS [MediumUrl],
    NULL AS [ThumbUrl],
    ROW_NUMBER() OVER (PARTITION BY t.[RentalCommercialListId] ORDER BY v.n) AS [SortOrder],
    CASE WHEN ROW_NUMBER() OVER (PARTITION BY t.[RentalCommercialListId] ORDER BY v.n) = 1 THEN 1 ELSE 0 END AS [IsCover],
    NULL AS [AltText],
    NULL AS [Width],
    NULL AS [Height],
    NULL AS [StoragePath],
    SYSUTCDATETIME() AS [CreatedAtUtc]
FROM [rentalCommercialPropertyListings] AS t
CROSS APPLY (VALUES
    (1, t.[PropImgUrl1]),
    (2, t.[PropImgUrl2]),
    (3, t.[PropImgUrl3]),
    (4, t.[PropImgUrl4]),
    (5, t.[PropImgUrl5]),
    (6, t.[PropImgUrl6]),
    (7, t.[PropImgUrl7]),
    (8, t.[PropImgUrl8]),
    (9, t.[PropImgUrl9]),
    (10, t.[PropImgUrl10]),
    (11, t.[PropImgUrl11]),
    (12, t.[PropImgUrl12]),
    (13, t.[PropImgUrl13]),
    (14, t.[PropImgUrl14]),
    (15, t.[PropImgUrl15]),
    (16, t.[PropImgUrl16]),
    (17, t.[PropImgUrl17]),
    (18, t.[PropImgUrl18]),
    (19, t.[PropImgUrl19]),
    (20, t.[PropImgUrl20]),
    (21, t.[PropImgUrl21]),
    (22, t.[PropImgUrl22]),
    (23, t.[PropImgUrl23]),
    (24, t.[PropImgUrl24]),
    (25, t.[PropImgUrl25]),
    (26, t.[PropImgUrl26]),
    (27, t.[PropImgUrl27]),
    (28, t.[PropImgUrl28]),
    (29, t.[PropImgUrl29]),
    (30, t.[PropImgUrl30])
) AS v(n, Url)
WHERE v.Url IS NOT NULL
  AND LTRIM(RTRIM(v.Url)) <> ''
  AND LTRIM(RTRIM(v.Url)) <> 'string'
  AND NOT EXISTS (
      SELECT 1 FROM [ListingImages] AS img
      WHERE img.[ListingTypeId] = 9 AND img.[ListingId] = t.[RentalCommercialListId]
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingImages");
        }
    }
}

