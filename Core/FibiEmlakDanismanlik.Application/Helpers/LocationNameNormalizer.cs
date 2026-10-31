using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class LocationNameNormalizer
    {
        private static readonly CultureInfo TrCulture = new("tr-TR");

        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var cleaned = Regex.Replace(value.Trim(), @"\s+", " ");
            var upper = cleaned.ToUpper(TrCulture);

            var sb = new StringBuilder(upper.Length);
            foreach (var ch in upper)
            {
                sb.Append(ch switch
                {
                    'İ' => 'I',
                    'Ş' => 'S',
                    'Ğ' => 'G',
                    'Ü' => 'U',
                    'Ö' => 'O',
                    'Ç' => 'C',
                    'Â' => 'A',
                    'Î' => 'I',
                    'Û' => 'U',
                    _ => ch
                });
            }

            return sb.ToString();
        }

        public static string NormalizeNeighborhood(string? value)
        {
            var normalized = Normalize(value);
            if (string.IsNullOrEmpty(normalized))
                return string.Empty;

            if (normalized.EndsWith(" MAHALLESI", StringComparison.Ordinal))
            {
                normalized = normalized.Substring(0, normalized.Length - " MAHALLESI".Length);
            }
            else if (normalized.EndsWith(" MAH.", StringComparison.Ordinal))
            {
                normalized = normalized.Substring(0, normalized.Length - " MAH.".Length);
            }
            else if (normalized.EndsWith(" MAH", StringComparison.Ordinal))
            {
                normalized = normalized.Substring(0, normalized.Length - " MAH".Length);
            }

            return normalized.Trim();
        }
    }
}
