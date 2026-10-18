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
    }
}

