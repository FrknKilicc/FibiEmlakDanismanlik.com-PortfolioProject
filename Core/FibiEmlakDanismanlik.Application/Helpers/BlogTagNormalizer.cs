using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class BlogTagNormalizer
    {
        private static readonly Regex MultiSpaceRegex = new(@"\s+", RegexOptions.Compiled);

        public static List<string> Normalize(IEnumerable<string>? tags)
        {
            if (tags == null)
            {
                return new List<string>();
            }

            var result = new List<string>();
            var trCulture = new CultureInfo("tr-TR");
            var seen = new HashSet<string>(StringComparer.Create(trCulture, true));

            foreach (var raw in tags)
            {
                if (string.IsNullOrWhiteSpace(raw))
                {
                    continue;
                }

                var cleaned = MultiSpaceRegex.Replace(raw.Trim(), " ");
                if (string.IsNullOrWhiteSpace(cleaned))
                {
                    continue;
                }

                if (seen.Add(cleaned))
                {
                    result.Add(cleaned);
                }
            }

            return result;
        }
    }
}
