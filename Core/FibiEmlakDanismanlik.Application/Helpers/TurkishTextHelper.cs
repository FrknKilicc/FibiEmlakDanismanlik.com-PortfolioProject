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
    }
}

