using System;
using FibiEmlakDanismanlik.Application.Helpers;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class IbbSeismicCsvParserTests
    {
        private const string ValidScenarioCsv =
@"ilce_adi;mahalle_adi;mahalle_koy_uavt;cok_agir_hasarli_bina_sayisi;agir_hasarli_bina_sayisi;orta_hasarli_bina_sayisi;hafif_hasarli_bina_sayisi;can_kaybi_sayisi;gecici_barinma
KADIKÖY;FENERBAHÇE MAHALLESİ;1001;12;45;120;300;5;150
BEŞİKTAŞ;BEBEK MAHALLESİ;1002;3;10;40;95;1;35";

        private const string ValidBuildingCsv =
@"mahalle_uavt;1980_oncesi;1980-2000_arasi;2000_sonrasi
1001;200;450;600
1002;150;300;250";

        [Fact]
        public void Parse_ValidCsvs_ReturnsJoinedRecords()
        {
            // Senaryo ve bina sayısı CSV'leri UAVT koduna göre iç birleşimle birleştirilmeli
            var records = IbbSeismicCsvParser.Parse(ValidScenarioCsv, ValidBuildingCsv);

            Assert.NotNull(records);
            Assert.Equal(2, records.Count);

            var first = records[0];
            Assert.Equal("KADIKÖY", first.DistrictName);
            Assert.Equal("FENERBAHÇE MAHALLESİ", first.NeighborhoodName);
            Assert.Equal(1001, first.UavtCode);
            Assert.Equal(200, first.BuildingsBefore1980Count);
            Assert.Equal(450, first.Buildings1980To2000Count);
            Assert.Equal(600, first.BuildingsAfter2000Count);
            Assert.Equal(12, first.VeryHeavilyDamagedBuildingCount);
            Assert.Equal(45, first.HeavilyDamagedBuildingCount);
            Assert.Equal(120, first.ModeratelyDamagedBuildingCount);
            Assert.Equal(300, first.LightlyDamagedBuildingCount);
            Assert.Equal(5, first.EstimatedFatalityCount);
            Assert.Equal(150, first.TemporaryShelterNeedCount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyOrWhitespaceScenarioCsv_ThrowsFormatException(string? scenario)
        {
            // Senaryo CSV boş olduğunda FormatException fırlatılmalı
            var ex = Assert.Throws<FormatException>(() => IbbSeismicCsvParser.Parse(scenario!, ValidBuildingCsv));
            Assert.Contains("Senaryo CSV içeriği boş", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyOrWhitespaceBuildingCsv_ThrowsFormatException(string? building)
        {
            // Bina sayısı CSV boş olduğunda FormatException fırlatılmalı
            var ex = Assert.Throws<FormatException>(() => IbbSeismicCsvParser.Parse(ValidScenarioCsv, building!));
            Assert.Contains("Bina sayısı CSV içeriği boş", ex.Message);
        }

        [Fact]
        public void Parse_MissingRequiredHeaderInBuildingCsv_ThrowsFormatException()
        {
            // Bina sayısı CSV'sinde zorunlu bir başlık eksikse FormatException fırlatılmalı
            var brokenBuildingCsv = @"mahalle_uavt;1980_oncesi;1980-2000_arasi
1001;200;450";

            var ex = Assert.Throws<FormatException>(() => IbbSeismicCsvParser.Parse(ValidScenarioCsv, brokenBuildingCsv));
            Assert.Contains("2000_sonrasi", ex.Message);
        }

        [Fact]
        public void Parse_MissingRequiredHeaderInScenarioCsv_ThrowsFormatException()
        {
            // Senaryo CSV'sinde zorunlu bir başlık eksikse FormatException fırlatılmalı
            var brokenScenarioCsv =
@"ilce_adi;mahalle_adi;mahalle_koy_uavt;cok_agir_hasarli_bina_sayisi;agir_hasarli_bina_sayisi;orta_hasarli_bina_sayisi;hafif_hasarli_bina_sayisi;can_kaybi_sayisi
KADIKÖY;FENERBAHÇE MAHALLESİ;1001;12;45;120;300;5";

            var ex = Assert.Throws<FormatException>(() => IbbSeismicCsvParser.Parse(brokenScenarioCsv, ValidBuildingCsv));
            Assert.Contains("gecici_barinma", ex.Message);
        }

        [Fact]
        public void Parse_UnmatchedUavt_SkipsUnmatchedRows()
        {
            // Bina sayısı CSV'sinde UAVT karşılığı bulunmayan senaryo satırları atlanmalı
            var scenarioCsvWithUnmatched =
@"ilce_adi;mahalle_adi;mahalle_koy_uavt;cok_agir_hasarli_bina_sayisi;agir_hasarli_bina_sayisi;orta_hasarli_bina_sayisi;hafif_hasarli_bina_sayisi;can_kaybi_sayisi;gecici_barinma
KADIKÖY;FENERBAHÇE MAHALLESİ;1001;12;45;120;300;5;150
ŞİŞLİ;MECİDİYEKÖY MAHALLESİ;9999;10;20;30;40;2;50";

            var records = IbbSeismicCsvParser.Parse(scenarioCsvWithUnmatched, ValidBuildingCsv);

            Assert.Single(records);
            Assert.Equal(1001, records[0].UavtCode);
        }

        [Fact]
        public void Parse_HandlesQuotesAndThousandSeparators_CorrectlyParsesNumbers()
        {
            // Noktalı binlik ayırıcılar ve tırnak işaretleri temizlenip sayıya dönüştürülmeli
            var scenarioWithSeparators =
@"ilce_adi;mahalle_adi;mahalle_koy_uavt;cok_agir_hasarli_bina_sayisi;agir_hasarli_bina_sayisi;orta_hasarli_bina_sayisi;hafif_hasarli_bina_sayisi;can_kaybi_sayisi;gecici_barinma
""KADIKÖY"";""FENERBAHÇE MAHALLESİ"";1001;""12"";""45"";""1.200"";""3.500"";""5"";""1.150""";

            var buildingWithSeparators =
@"mahalle_uavt;1980_oncesi;1980-2000_arasi;2000_sonrasi
1001;""1.200"";""2.450"";""3.600""";

            var records = IbbSeismicCsvParser.Parse(scenarioWithSeparators, buildingWithSeparators);

            Assert.Single(records);
            var record = records[0];
            Assert.Equal("KADIKÖY", record.DistrictName);
            Assert.Equal(1200, record.ModeratelyDamagedBuildingCount);
            Assert.Equal(3500, record.LightlyDamagedBuildingCount);
            Assert.Equal(1150, record.TemporaryShelterNeedCount);
            Assert.Equal(1200, record.BuildingsBefore1980Count);
            Assert.Equal(2450, record.Buildings1980To2000Count);
            Assert.Equal(3600, record.BuildingsAfter2000Count);
        }

        [Fact]
        public void Parse_CorruptedRowWithMissingColumns_SkipsRowAndParsesValidRows()
        {
            // Eksik sütunlu bozuk veri satırları sessizce atlanmalı ve geçerli satırlar işlenmeli
            var scenarioWithCorruptedRow =
@"ilce_adi;mahalle_adi;mahalle_koy_uavt;cok_agir_hasarli_bina_sayisi;agir_hasarli_bina_sayisi;orta_hasarli_bina_sayisi;hafif_hasarli_bina_sayisi;can_kaybi_sayisi;gecici_barinma
BOZUK_SATIR;EKSİK_SUTUN
KADIKÖY;FENERBAHÇE MAHALLESİ;1001;12;45;120;300;5;150";

            var records = IbbSeismicCsvParser.Parse(scenarioWithCorruptedRow, ValidBuildingCsv);

            Assert.Single(records);
            Assert.Equal(1001, records[0].UavtCode);
        }
    }
}
