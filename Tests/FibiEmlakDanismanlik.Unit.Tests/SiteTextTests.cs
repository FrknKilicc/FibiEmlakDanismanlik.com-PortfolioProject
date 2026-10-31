using System.Collections.Generic;
using System.Linq;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.SiteTextCommands;
using FibiEmlakDanismanlik.Application.Features.Validators.SiteTextValidators;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class SiteTextTests
    {
        [Fact]
        public void Definitions_AllKeys_AreUnique()
        {
            // Tanımlı tüm site metni anahtarları benzersiz olmalıdır
            var allKeys = SiteTextDefinitions.All.Select(d => d.Key).ToList();

            Assert.NotEmpty(allKeys);
            Assert.Equal(allKeys.Count, allKeys.Distinct().Count());
        }

        [Fact]
        public void Definitions_NumericDefaults_AreWithinMinMaxRange()
        {
            // Sayı türündeki tüm varsayılan değerler tanımlı Min ve Max aralığında olmalıdır
            var numberDefs = SiteTextDefinitions.All.Where(d => d.Kind == SiteTextDefinitions.KindNumber);

            Assert.All(numberDefs, def =>
            {
                var parsed = int.TryParse(def.Default, out var val);
                Assert.True(parsed, $"{def.Key} varsayılan sayısal değeri geçerli bir tamsayı değil.");
                Assert.InRange(val, def.Min, def.Max);
            });
        }

        [Fact]
        public void Definitions_TextDefaults_DoNotExceedMaxLength()
        {
            // Metin türündeki varsayılan değerler tanımlı MaxLength sınırını aşmamalıdır
            var textDefs = SiteTextDefinitions.All.Where(d => d.Kind != SiteTextDefinitions.KindNumber);

            Assert.All(textDefs, def =>
            {
                Assert.True(def.Default.Length <= def.MaxLength,
                    $"{def.Key} varsayılan metni ({def.Default.Length} kar.), izin verilen sınırı ({def.MaxLength} kar.) aşıyor.");
            });
        }

        [Fact]
        public void Find_ExistingAndNonExistingKeys_ReturnsExpected()
        {
            // Find metodu mevcut anahtarda tanımı, olmayan anahtarda null dönmelidir
            var found = SiteTextDefinitions.Find("home.latest.count");
            var notFound = SiteTextDefinitions.Find("olmayan.tanimsiz.anahtar");

            Assert.NotNull(found);
            Assert.Equal("home.latest.count", found.Key);
            Assert.Null(notFound);
        }

        [Fact]
        public void Merge_OverwritesDefaultsWithStoredAndDiscardsUnknownKeys()
        {
            // Merge, kayıtlı değerleri varsayılanların üzerine yazmalı ve tanımsız anahtarları atmalıdır
            var stored = new Dictionary<string, string>
            {
                ["home.latest.count"] = "18",
                ["gecersiz.anahtar"] = "yok_sayilacak_deger"
            };

            var merged = SiteTextDefinitions.Merge(stored);

            // Kayıtlı değer üzerine yazılmış olmalı
            Assert.Equal("18", merged["home.latest.count"]);

            // Tanımsız anahtar sonuca dahil edilmemeli
            Assert.False(merged.ContainsKey("gecersiz.anahtar"));

            // Gönderilmeyen diğer anahtarlar varsayılan değerlerini korumalı
            var bannerSubtitleDef = SiteTextDefinitions.Find("home.banner.subtitle");
            Assert.NotNull(bannerSubtitleDef);
            Assert.Equal(bannerSubtitleDef.Default, merged["home.banner.subtitle"]);
        }

        [Fact]
        public void Validator_ValidValues_PassesValidation()
        {
            // Geçerli metin ve geçerli sayı içeren güncelleme komutu doğrulamadan geçmelidir
            var validator = new UpdateSiteTextsCommandValidator();
            var command = new UpdateSiteTextsCommand
            {
                Values = new Dictionary<string, string?>
                {
                    ["home.latest.title"] = "Yeni İlanlar Başlığı",
                    ["home.latest.count"] = "12"
                }
            };

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validator_UnknownKey_FailsValidation()
        {
            // Tanımsız anahtar gönderildiğinde doğrulama hatası üretilmelidir
            var validator = new UpdateSiteTextsCommandValidator();
            var command = new UpdateSiteTextsCommand
            {
                Values = new Dictionary<string, string?>
                {
                    ["bilinmeyen.anahtar"] = "deneme"
                }
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Bilinmeyen site metni: bilinmeyen.anahtar"));
        }

        [Fact]
        public void Validator_TextExceedingMaxLength_FailsValidation()
        {
            // Tanımlı azami uzunluğu aşan metin değeri reddedilmelidir
            var validator = new UpdateSiteTextsCommandValidator();
            // home.latest.eyebrow MaxLength = 60
            var command = new UpdateSiteTextsCommand
            {
                Values = new Dictionary<string, string?>
                {
                    ["home.latest.eyebrow"] = new string('X', 61)
                }
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("en fazla 60 karakter olabilir"));
        }

        [Theory]
        [InlineData("25")]           // Max 24'ü aşıyor
        [InlineData("-1")]           // Min 0'ın altında
        [InlineData("metin_sayi_degil")] // Sayıya çevrilemeyen metin
        public void Validator_NumberOutOfRangeOrNonNumeric_FailsValidation(string invalidValue)
        {
            // Min-Max aralığı dışındaki veya tam sayı olmayan değerler reddedilmelidir
            var validator = new UpdateSiteTextsCommandValidator();
            var command = new UpdateSiteTextsCommand
            {
                Values = new Dictionary<string, string?>
                {
                    ["home.latest.count"] = invalidValue
                }
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("0 ile 24 arasında bir tam sayı olmalıdır"));
        }

        [Fact]
        public void Validator_NullValues_FailsValidation()
        {
            // Values sözlüğü null olduğunda doğrulama hatası üretilmelidir
            var validator = new UpdateSiteTextsCommandValidator();
            var command = new UpdateSiteTextsCommand
            {
                Values = null!
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Güncellenecek metin gönderilmelidir"));
        }
    }
}
