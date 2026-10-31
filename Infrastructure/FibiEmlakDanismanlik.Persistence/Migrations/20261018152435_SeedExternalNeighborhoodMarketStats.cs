using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedExternalNeighborhoodMarketStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Değerler 2026 Ekim için tahmini demo verisidir, gerçek piyasa verisiyle güncellenmelidir; mahalleler ada göre eşleşir.
            migrationBuilder.Sql("""
                -- Değerler 2026 Ekim için tahmini demo verisidir, gerçek piyasa verisiyle güncellenmelidir; mahalleler ada göre eşleşir.
                INSERT INTO ExternalNeighborhoodMarketStats
                    (NeighborhoodId, BaseSalePricePerSqm, BaseRentPricePerSqm, AgencyMarginMultiplier, BandSpreadPercentage,
                     Last1YearGrowthRate, Last3YearsCompoundGrowthRate, RegionalDevelopmentFactor, SoilRiskClass,
                     UrbanTransformationAppetiteScore, HistoricalQuarterlyPricesJson, LastUpdatedDate)
                SELECT n.NeighborhoodId, v.Sale, v.Rent, 1.08, 15, v.Growth1, v.Growth3, v.DevFactor, v.Soil, v.Appetite, v.History, '2026-10-01'
                FROM (VALUES
                    (N'ZEYTİNBURNU', N'BEŞTELSİZ MAHALLESİ', 76000, 333, 34, 128, 1.30, 3, 84, N'[{"quarter":"2024-Q4","price":46500},{"quarter":"2025-Q1","price":49700},{"quarter":"2025-Q2","price":53100},{"quarter":"2025-Q3","price":56700},{"quarter":"2025-Q4","price":61000},{"quarter":"2026-Q1","price":65700},{"quarter":"2026-Q2","price":70600},{"quarter":"2026-Q3","price":76000}]'),
                    (N'ZEYTİNBURNU', N'ÇIRPICI MAHALLESİ', 71000, 329, 33, 124, 1.25, 3, 82, N'[{"quarter":"2024-Q4","price":43900},{"quarter":"2025-Q1","price":46900},{"quarter":"2025-Q2","price":50000},{"quarter":"2025-Q3","price":53400},{"quarter":"2025-Q4","price":57300},{"quarter":"2026-Q1","price":61600},{"quarter":"2026-Q2","price":66100},{"quarter":"2026-Q3","price":71000}]'),
                    (N'ZEYTİNBURNU', N'VELİEFENDİ MAHALLESİ', 86000, 341, 36, 135, 1.35, 2, 78, N'[{"quarter":"2024-Q4","price":51500},{"quarter":"2025-Q1","price":55200},{"quarter":"2025-Q2","price":59100},{"quarter":"2025-Q3","price":63200},{"quarter":"2025-Q4","price":68300},{"quarter":"2026-Q1","price":73700},{"quarter":"2026-Q2","price":79600},{"quarter":"2026-Q3","price":86000}]'),
                    (N'ZEYTİNBURNU', N'KAZLIÇEŞME MAHALLESİ', 98000, 355, 40, 150, 1.45, 3, 88, N'[{"quarter":"2024-Q4","price":56300},{"quarter":"2025-Q1","price":60600},{"quarter":"2025-Q2","price":65100},{"quarter":"2025-Q3","price":70000},{"quarter":"2025-Q4","price":76100},{"quarter":"2026-Q1","price":82800},{"quarter":"2026-Q2","price":90100},{"quarter":"2026-Q3","price":98000}]'),
                    (N'BAKIRKÖY', N'ATAKÖY 1. KISIM MAHALLESİ', 132000, 458, 30, 118, 1.15, 3, 62, N'[{"quarter":"2024-Q4","price":83600},{"quarter":"2025-Q1","price":89200},{"quarter":"2025-Q2","price":95200},{"quarter":"2025-Q3","price":101500},{"quarter":"2025-Q4","price":108400},{"quarter":"2026-Q1","price":115800},{"quarter":"2026-Q2","price":123600},{"quarter":"2026-Q3","price":132000}]'),
                    (N'BAKIRKÖY', N'KARTALTEPE MAHALLESİ', 96000, 381, 32, 122, 1.25, 2, 80, N'[{"quarter":"2024-Q4","price":59800},{"quarter":"2025-Q1","price":63900},{"quarter":"2025-Q2","price":68200},{"quarter":"2025-Q3","price":72700},{"quarter":"2025-Q4","price":78000},{"quarter":"2026-Q1","price":83600},{"quarter":"2026-Q2","price":89600},{"quarter":"2026-Q3","price":96000}]'),
                    (N'BAKIRKÖY', N'OSMANİYE MAHALLESİ', 102000, 386, 31, 120, 1.20, 2, 76, N'[{"quarter":"2024-Q4","price":64100},{"quarter":"2025-Q1","price":68400},{"quarter":"2025-Q2","price":73000},{"quarter":"2025-Q3","price":77900},{"quarter":"2025-Q4","price":83300},{"quarter":"2026-Q1","price":89100},{"quarter":"2026-Q2","price":95300},{"quarter":"2026-Q3","price":102000}]'),
                    (N'BAKIRKÖY', N'YEŞİLKÖY MAHALLESİ', 158000, 506, 28, 112, 1.10, 2, 48, N'[{"quarter":"2024-Q4","price":102200},{"quarter":"2025-Q1","price":108800},{"quarter":"2025-Q2","price":115900},{"quarter":"2025-Q3","price":123400},{"quarter":"2025-Q4","price":131300},{"quarter":"2026-Q1","price":139700},{"quarter":"2026-Q2","price":148500},{"quarter":"2026-Q3","price":158000}]'),
                    (N'BAHÇELİEVLER', N'BAHÇELİEVLER MAHALLESİ', 73000, 304, 31, 121, 1.20, 2, 74, N'[{"quarter":"2024-Q4","price":45800},{"quarter":"2025-Q1","price":48900},{"quarter":"2025-Q2","price":52200},{"quarter":"2025-Q3","price":55700},{"quarter":"2025-Q4","price":59600},{"quarter":"2026-Q1","price":63800},{"quarter":"2026-Q2","price":68200},{"quarter":"2026-Q3","price":73000}]'),
                    (N'BAHÇELİEVLER', N'ŞİRİNEVLER MAHALLESİ', 56000, 275, 29, 115, 1.15, 2, 79, N'[{"quarter":"2024-Q4","price":35800},{"quarter":"2025-Q1","price":38200},{"quarter":"2025-Q2","price":40700},{"quarter":"2025-Q3","price":43400},{"quarter":"2025-Q4","price":46300},{"quarter":"2026-Q1","price":49300},{"quarter":"2026-Q2","price":52500},{"quarter":"2026-Q3","price":56000}]'),
                    (N'BAHÇELİEVLER', N'SİYAVUŞPAŞA MAHALLESİ', 54000, 281, 28, 112, 1.10, 2, 77, N'[{"quarter":"2024-Q4","price":34900},{"quarter":"2025-Q1","price":37200},{"quarter":"2025-Q2","price":39600},{"quarter":"2025-Q3","price":42200},{"quarter":"2025-Q4","price":44900},{"quarter":"2026-Q1","price":47700},{"quarter":"2026-Q2","price":50800},{"quarter":"2026-Q3","price":54000}]'),
                    (N'BAHÇELİEVLER', N'YENİBOSNA MERKEZ MAHALLESİ', 59000, 289, 33, 126, 1.30, 3, 81, N'[{"quarter":"2024-Q4","price":36400},{"quarter":"2025-Q1","price":38900},{"quarter":"2025-Q2","price":41500},{"quarter":"2025-Q3","price":44400},{"quarter":"2025-Q4","price":47600},{"quarter":"2026-Q1","price":51200},{"quarter":"2026-Q2","price":54900},{"quarter":"2026-Q3","price":59000}]'),
                    (N'FATİH', N'AKSARAY MAHALLESİ', 61000, 318, 26, 108, 1.05, 4, 52, N'[{"quarter":"2024-Q4","price":40100},{"quarter":"2025-Q1","price":42700},{"quarter":"2025-Q2","price":45500},{"quarter":"2025-Q3","price":48400},{"quarter":"2025-Q4","price":51300},{"quarter":"2026-Q1","price":54300},{"quarter":"2026-Q2","price":57600},{"quarter":"2026-Q3","price":61000}]'),
                    (N'FATİH', N'BALAT MAHALLESİ', 77000, 292, 35, 132, 1.25, 2, 30, N'[{"quarter":"2024-Q4","price":46600},{"quarter":"2025-Q1","price":49800},{"quarter":"2025-Q2","price":53300},{"quarter":"2025-Q3","price":57000},{"quarter":"2025-Q4","price":61500},{"quarter":"2026-Q1","price":66300},{"quarter":"2026-Q2","price":71400},{"quarter":"2026-Q3","price":77000}]'),
                    (N'FATİH', N'ŞEHREMİNİ MAHALLESİ', 62000, 287, 27, 110, 1.10, 3, 58, N'[{"quarter":"2024-Q4","price":40400},{"quarter":"2025-Q1","price":43100},{"quarter":"2025-Q2","price":45800},{"quarter":"2025-Q3","price":48800},{"quarter":"2025-Q4","price":51800},{"quarter":"2026-Q1","price":55000},{"quarter":"2026-Q2","price":58400},{"quarter":"2026-Q3","price":62000}]'),
                    (N'FATİH', N'KOCA MUSTAFAPAŞA MAHALLESİ', 64000, 296, 29, 114, 1.10, 3, 61, N'[{"quarter":"2024-Q4","price":41000},{"quarter":"2025-Q1","price":43700},{"quarter":"2025-Q2","price":46600},{"quarter":"2025-Q3","price":49600},{"quarter":"2025-Q4","price":52900},{"quarter":"2026-Q1","price":56300},{"quarter":"2026-Q2","price":60100},{"quarter":"2026-Q3","price":64000}]')
                ) AS v(District, Neighborhood, Sale, Rent, Growth1, Growth3, DevFactor, Soil, Appetite, History)
                JOIN Cities c ON c.PlaceCode = 34
                JOIN Districts d ON d.CityId = c.CityId AND d.Name = v.District
                JOIN Neighborhoods n ON n.DistrictId = d.DistrictId AND n.Name = v.Neighborhood
                WHERE NOT EXISTS (SELECT 1 FROM ExternalNeighborhoodMarketStats s WHERE s.NeighborhoodId = n.NeighborhoodId);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE s
                FROM ExternalNeighborhoodMarketStats s
                JOIN Neighborhoods n ON s.NeighborhoodId = n.NeighborhoodId
                JOIN Districts d ON n.DistrictId = d.DistrictId
                JOIN Cities c ON d.CityId = c.CityId AND c.PlaceCode = 34
                JOIN (VALUES
                    (N'ZEYTİNBURNU', N'BEŞTELSİZ MAHALLESİ'),
                    (N'ZEYTİNBURNU', N'ÇIRPICI MAHALLESİ'),
                    (N'ZEYTİNBURNU', N'VELİEFENDİ MAHALLESİ'),
                    (N'ZEYTİNBURNU', N'KAZLIÇEŞME MAHALLESİ'),
                    (N'BAKIRKÖY', N'ATAKÖY 1. KISIM MAHALLESİ'),
                    (N'BAKIRKÖY', N'KARTALTEPE MAHALLESİ'),
                    (N'BAKIRKÖY', N'OSMANİYE MAHALLESİ'),
                    (N'BAKIRKÖY', N'YEŞİLKÖY MAHALLESİ'),
                    (N'BAHÇELİEVLER', N'BAHÇELİEVLER MAHALLESİ'),
                    (N'BAHÇELİEVLER', N'ŞİRİNEVLER MAHALLESİ'),
                    (N'BAHÇELİEVLER', N'SİYAVUŞPAŞA MAHALLESİ'),
                    (N'BAHÇELİEVLER', N'YENİBOSNA MERKEZ MAHALLESİ'),
                    (N'FATİH', N'AKSARAY MAHALLESİ'),
                    (N'FATİH', N'BALAT MAHALLESİ'),
                    (N'FATİH', N'ŞEHREMİNİ MAHALLESİ'),
                    (N'FATİH', N'KOCA MUSTAFAPAŞA MAHALLESİ')
                ) AS v(District, Neighborhood)
                    ON d.Name = v.District AND n.Name = v.Neighborhood;
                """);
        }
    }
}
