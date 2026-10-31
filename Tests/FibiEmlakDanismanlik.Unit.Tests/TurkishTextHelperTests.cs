using FibiEmlakDanismanlik.Application.Helpers;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class TurkishTextHelperTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\r\n")]
        public void ToTurkishTitleCase_NullOrWhiteSpace_ReturnsEmpty(string? input)
        {
            // Boş veya yalnızca boşluk içeren girdilerde boş string dönmelidir
            var result = TurkishTextHelper.ToTurkishTitleCase(input);

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("istanbul", "İstanbul")]
        [InlineData("IĞDIR", "Iğdır")]
        [InlineData("şemsiye ve çanta", "Şemsiye Ve Çanta")]
        [InlineData("çanakkale boğazı", "Çanakkale Boğazı")]
        [InlineData("ömer seyfettin", "Ömer Seyfettin")]
        public void ToTurkishTitleCase_TurkishCharacters_ConvertsCorrectly(string input, string expected)
        {
            // Türkçe karakterlerin (i/İ, ı/I, ş, ç, ö, ğ) baş harfleri büyütülmeli
            var result = TurkishTextHelper.ToTurkishTitleCase(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToTurkishTitleCase_LeadingAndTrailingSpaces_TrimsAndConverts()
        {
            // Baş ve sondaki boşluklar kırpılmalıdır
            var result = TurkishTextHelper.ToTurkishTitleCase("   kadıköy moda   ");

            Assert.Equal("Kadıköy Moda", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ToComparisonKey_NullOrWhiteSpace_ReturnsEmpty(string? input)
        {
            // Boş girdilerde boş string dönmelidir
            var result = TurkishTextHelper.ToComparisonKey(input);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ToComparisonKey_CircumflexAndMultipleSpaces_NormalizesAndCollapses()
        {
            // Düzeltme işaretleri (â->A, î->İ, û->U) dönüştürülmeli ve ardışık boşluklar teke indirilmeli
            var result = TurkishTextHelper.ToComparisonKey("  kâzım    resmî   sükût  ");

            Assert.Equal("KAZIM RESMİ SÜKUT", result);
        }

        [Theory]
        [InlineData("ışık", "IŞIK")]
        [InlineData("iğde", "İĞDE")]
        [InlineData("şule", "ŞULE")]
        [InlineData("özlem", "ÖZLEM")]
        public void ToComparisonKey_TurkishDottedAndDotlessI_PreservesTurkishCasing(string input, string expected)
        {
            // Türkçe büyük harf kuralına göre ı->I ve i->İ korunmalıdır
            var result = TurkishTextHelper.ToComparisonKey(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ToCompactComparisonKey_NullOrWhiteSpace_ReturnsEmpty(string? input)
        {
            // Boş girdilerde boş string dönmelidir
            var result = TurkishTextHelper.ToCompactComparisonKey(input);

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("Şişli - Merkez.", "SISLIMERKEZ")]
        [InlineData("Çankaya / Örnek.", "CANKAYA/ORNEK")]
        [InlineData("Gazi   Osman - Paşa.", "GAZIOSMANPASA")]
        [InlineData("Ağrı-Dağı..", "AGRIDAGI")]
        [InlineData("Kadıköy   Moda", "KADIKOYMODA")]
        public void ToCompactComparisonKey_TurkishCharsAndPunctuation_FoldsToAsciiAndRemovesSpacesPunctuation(string input, string expected)
        {
            // Türkçe karakterler ASCII'ye katlanmalı; boşluk, tire ve nokta kaldırılmalıdır
            var result = TurkishTextHelper.ToCompactComparisonKey(input);

            Assert.Equal(expected, result);
        }
    }
}
