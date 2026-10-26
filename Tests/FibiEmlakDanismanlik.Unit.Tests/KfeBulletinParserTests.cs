using FibiEmlakDanismanlik.Application.Helpers;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class KfeBulletinParserTests
    {
        private const string SampleValidBulletin = @"
2024 yılı Mayıs ayında bir önceki aya göre yüzde 1,5 oranında artan KFE, bir önceki yılın aynı ayına göre nominal olarak yüzde 45,0 oranında artmış bulunmaktadır.
2024 yılı Mayıs ayında bir önceki aya göre yüzde 2,1 oranında artan YKKE, bir önceki yılın aynı ayına göre nominal olarak yüzde 55,4 oranında artmış bulunmaktadır.

Bölgelere Göre Konut Fiyat Endeksi
Türkiye 45,0
İstanbul 45,2
Ankara 48,1
İzmir 42,0
Bursa, Eskişehir, Bilecik 39,5
Antalya, Isparta, Burdur 35,4
Adana, Mersin 40,1
Konya, Karaman 38,2
Trabzon, Ordu, Giresun, Rize, Artvin, Gümüşhane 41,7
Gaziantep, Adıyaman, Kilis 43,8
Diyarbakır, Şanlıurfa 44,6
=====

Bölgelere Göre Yeni Kiracı Kira Endeksi
Türkiye 55,4
İstanbul 55,2
Ankara 58,1
İzmir 52,0
Bursa, Eskişehir, Bilecik 49,5
Antalya, Isparta, Burdur 45,4
Adana, Mersin 50,1
Konya, Karaman 48,2
Trabzon, Ordu, Giresun, Rize, Artvin, Gümüşhane 51,7
Gaziantep, Adıyaman, Kilis 53,8
Diyarbakır, Şanlıurfa 54,6
=====
";

        [Fact]
        public void Parse_ValidBulletinText_ReturnsParsedData()
        {
            // Geçerli TCMB KFE ve YKKE bülten metnini eksiksiz ayrıştırmalıdır
            var result = KfeBulletinParser.Parse(SampleValidBulletin);

            Assert.NotNull(result);
            Assert.Equal("2024-05", result.Period);
            Assert.Equal(1.5m, result.NationalMonthlyPriceChange);
            Assert.Equal(45.0m, result.NationalAnnualPriceChange);
            Assert.Equal(2.1m, result.NationalMonthlyRentChange);
            Assert.Equal(55.4m, result.NationalAnnualRentChange);

            Assert.Equal(10, result.RegionalPriceChanges.Count);
            Assert.Equal(10, result.RegionalRentChanges.Count);

            // İlk bölge İstanbul
            var istanbulPrice = result.RegionalPriceChanges[0];
            Assert.Contains("İSTANBUL", istanbulPrice.ProvinceKeys);
            Assert.Equal(45.2m, istanbulPrice.AnnualChange);

            // Çoklu il bölgesi
            var bursaPrice = result.RegionalPriceChanges[3];
            Assert.Equal(3, bursaPrice.ProvinceKeys.Count);
            Assert.Contains("BURSA", bursaPrice.ProvinceKeys);
            Assert.Contains("ESKİŞEHİR", bursaPrice.ProvinceKeys);
            Assert.Contains("BİLECİK", bursaPrice.ProvinceKeys);
            Assert.Equal(39.5m, bursaPrice.AnnualChange);
        }

        [Fact]
        public void Parse_NegativeChanges_ParsesNegativeDecimals()
        {
            // 'azalan' ve 'azalmış' ifadelerinde değerler eksi olarak işaretlenmelidir
            var text = @"
2024 yılı Ocak ayında bir önceki aya göre yüzde 0,8 oranında azalan KFE, bir önceki yılın aynı ayına göre nominal olarak yüzde 12,3 oranında azalmış bulunmaktadır.
2024 yılı Ocak ayında bir önceki aya göre yüzde 1,2 oranında azalan YKKE, bir önceki yılın aynı ayına göre nominal olarak yüzde 5,5 oranında azalmış bulunmaktadır.

Bölgelere Göre Konut Fiyat Endeksi
İstanbul -12,3
Ankara 10,0
İzmir 11,0
Bursa 12,0
Antalya 13,0
Adana 14,0
Konya 15,0
Trabzon 16,0
Gaziantep 17,0
Diyarbakır 18,0
=====

Bölgelere Göre Yeni Kiracı Kira Endeksi
İstanbul -5,5
Ankara 8,0
İzmir 9,0
Bursa 10,0
Antalya 11,0
Adana 12,0
Konya 13,0
Trabzon 14,0
Gaziantep 15,0
Diyarbakır 16,0
=====
";
            var result = KfeBulletinParser.Parse(text);

            Assert.NotNull(result);
            Assert.Equal("2024-01", result.Period);
            Assert.Equal(-0.8m, result.NationalMonthlyPriceChange);
            Assert.Equal(-12.3m, result.NationalAnnualPriceChange);
            Assert.Equal(-1.2m, result.NationalMonthlyRentChange);
            Assert.Equal(-5.5m, result.NationalAnnualRentChange);
            Assert.Equal(-12.3m, result.RegionalPriceChanges[0].AnnualChange);
            Assert.Equal(-5.5m, result.RegionalRentChanges[0].AnnualChange);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_NullOrWhiteSpace_ReturnsNull(string? input)
        {
            // Boş girdilerde null dönmelidir
            var result = KfeBulletinParser.Parse(input!);

            Assert.Null(result);
        }

        [Fact]
        public void Parse_MissingYkkeSentence_ReturnsNull()
        {
            // YKKE cümlesi bültende yoksa ayrıştırma başarısız olmalı ve null dönmelidir
            var text = @"
2024 yılı Mayıs ayında bir önceki aya göre yüzde 1,5 oranında artan KFE, bir önceki yılın aynı ayına göre nominal olarak yüzde 45,0 oranında artmış bulunmaktadır.

Bölgelere Göre Konut Fiyat Endeksi
İstanbul 45,2
Ankara 48,1
İzmir 42,0
Bursa 39,5
Antalya 35,4
Adana 40,1
Konya 38,2
Trabzon 41,7
Gaziantep 43,8
Diyarbakır 44,6
=====
";
            var result = KfeBulletinParser.Parse(text);

            Assert.Null(result);
        }

        [Fact]
        public void Parse_MissingSectionHeader_ReturnsNull()
        {
            // Bölge tablosu başlığı bulunamazsa null dönmelidir
            var text = @"
2024 yılı Mayıs ayında bir önceki aya göre yüzde 1,5 oranında artan KFE, bir önceki yılın aynı ayına göre nominal olarak yüzde 45,0 oranında artmış bulunmaktadır.
2024 yılı Mayıs ayında bir önceki aya göre yüzde 2,1 oranında artan YKKE, bir önceki yılın aynı ayına göre nominal olarak yüzde 55,4 oranında artmış bulunmaktadır.

Bölgelere Göre Konut Fiyat Endeksi
İstanbul 45,2
Ankara 48,1
İzmir 42,0
Bursa 39,5
Antalya 35,4
Adana 40,1
Konya 38,2
Trabzon 41,7
Gaziantep 43,8
Diyarbakır 44,6
=====
";
            var result = KfeBulletinParser.Parse(text);

            Assert.Null(result);
        }

        [Fact]
        public void Parse_InsufficientRegionsCount_ReturnsNull()
        {
            // Bölge sayısı asgari sayıdan (10) az ise null dönmelidir
            var text = @"
2024 yılı Mayıs ayında bir önceki aya göre yüzde 1,5 oranında artan KFE, bir önceki yılın aynı ayına göre nominal olarak yüzde 45,0 oranında artmış bulunmaktadır.
2024 yılı Mayıs ayında bir önceki aya göre yüzde 2,1 oranında artan YKKE, bir önceki yılın aynı ayına göre nominal olarak yüzde 55,4 oranında artmış bulunmaktadır.

Bölgelere Göre Konut Fiyat Endeksi
İstanbul 45,2
Ankara 48,1
İzmir 42,0
=====

Bölgelere Göre Yeni Kiracı Kira Endeksi
İstanbul 55,2
Ankara 58,1
İzmir 52,0
=====
";
            var result = KfeBulletinParser.Parse(text);

            Assert.Null(result);
        }

        [Fact]
        public void Parse_PercentageExceedingMaxLimit_ReturnsNull()
        {
            // Azami değişim sınırını (yüzde 200) aşan değerler bülten hatası kabul edilip null dönmelidir
            var text = SampleValidBulletin.Replace("yüzde 45,0 oranında artmış", "yüzde 250,0 oranında artmış");

            var result = KfeBulletinParser.Parse(text);

            Assert.Null(result);
        }

        [Fact]
        public void Parse_TürkiyeLabelSkipped_OnlyValidProvincesReturned()
        {
            // 'TÜRKİYE' tekil etiketi bölge listesine dahil edilmemelidir
            var result = KfeBulletinParser.Parse(SampleValidBulletin);

            Assert.NotNull(result);
            Assert.DoesNotContain(result.RegionalPriceChanges, r => r.ProvinceKeys.Count == 1 && r.ProvinceKeys[0] == "TÜRKİYE");
            Assert.DoesNotContain(result.RegionalRentChanges, r => r.ProvinceKeys.Count == 1 && r.ProvinceKeys[0] == "TÜRKİYE");
        }
    }
}
