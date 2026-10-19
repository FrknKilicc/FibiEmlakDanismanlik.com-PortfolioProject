using System.Globalization;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class TurkishTextHelper
    {
        private static readonly CultureInfo TrCulture = new("tr-TR");

        public static string ToTurkishTitleCase(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var lower = value.Trim().ToLower(TrCulture);
            return TrCulture.TextInfo.ToTitleCase(lower);
        }

        /// <summary>Karşılaştırma anahtarı: Türkçe büyük harf, düzeltme işaretleri kaldırılmış (â→A, î→İ, û→U), fazla boşluk tek boşluk.</summary>
        public static string ToComparisonKey(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var upper = value.ToUpper(TrCulture)
                .Replace('Â', 'A')
                .Replace('Î', 'İ')
                .Replace('Û', 'U')
                .Trim();

            var parts = upper.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", parts);
        }

        /// <summary>Gevşek karşılaştırma anahtarı: ToComparisonKey sonucunda Türkçe harfler ASCII'ye katlanır (Ş→S, Ç→C, Ğ→G, İ→I, Ö→O, Ü→U), boşluk, tire ve nokta kaldırılır.</summary>
        public static string ToCompactComparisonKey(string? value)
        {
            var key = ToComparisonKey(value);
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return key
                .Replace('Ş', 'S')
                .Replace('Ç', 'C')
                .Replace('Ğ', 'G')
                .Replace('İ', 'I')
                .Replace('Ö', 'O')
                .Replace('Ü', 'U')
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty);
        }
    }
}

