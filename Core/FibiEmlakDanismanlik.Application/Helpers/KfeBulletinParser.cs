using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>TCMB Konut Fiyat Endeksi bülten metni ayrıştırıcısı.</summary>
    public static class KfeBulletinParser
    {
        private static readonly Regex KfeRegex = new(
            @"(\d{4})\s+yılı\s+(\p{L}+)\s+ayında\s+bir\s+önceki\s+aya\s+göre\s+yüzde\s+(\d+,\d+)\s+oranında\s+(artan|azalan)\s+KFE,\s+bir\s+önceki\s+yılın\s+aynı\s+ayına\s+göre\s+nominal\s+olarak\s+yüzde\s+(\d+,\d+)\s+oranında\s+(artmış|azalmış)",
            RegexOptions.Compiled);

        private static readonly Regex YkkeRegex = new(
            @"(\d{4})\s+yılı\s+(\p{L}+)\s+ayında\s+bir\s+önceki\s+aya\s+göre\s+yüzde\s+(\d+,\d+)\s+oranında\s+(artan|azalan)\s+YKKE,\s+bir\s+önceki\s+yılın\s+aynı\s+ayına\s+göre\s+nominal\s+olarak\s+yüzde\s+(\d+,\d+)\s+oranında\s+(artmış|azalmış)",
            RegexOptions.Compiled);

        public static KfeBulletinData? Parse(string bulletinText)
        {
            if (string.IsNullOrWhiteSpace(bulletinText))
                return null;

            try
            {
                // 1. Ulusal cümleleri oku
                var collapsed = Regex.Replace(bulletinText, @"\s+", " ");

                var kfeMatch = KfeRegex.Match(collapsed);
                if (!kfeMatch.Success)
                    return null;

                var ykkeMatch = YkkeRegex.Match(collapsed);
                if (!ykkeMatch.Success)
                    return null;

                var yearStr = kfeMatch.Groups[1].Value;
                var monthStr = kfeMatch.Groups[2].Value;
                var month = ParseTurkishMonth(monthStr);
                if (!month.HasValue || !int.TryParse(yearStr, out var year))
                    return null;

                var period = $"{year:D4}-{month.Value:D2}";

                if (!TryParseDecimal(kfeMatch.Groups[3].Value, out var kfeMonthly) ||
                    !TryParseDecimal(kfeMatch.Groups[5].Value, out var kfeAnnual) ||
                    !TryParseDecimal(ykkeMatch.Groups[3].Value, out var ykkeMonthly) ||
                    !TryParseDecimal(ykkeMatch.Groups[5].Value, out var ykkeAnnual))
                {
                    return null;
                }

                if (kfeMatch.Groups[4].Value == "azalan") kfeMonthly = -kfeMonthly;
                if (kfeMatch.Groups[6].Value == "azalmış") kfeAnnual = -kfeAnnual;
                if (ykkeMatch.Groups[4].Value == "azalan") ykkeMonthly = -ykkeMonthly;
                if (ykkeMatch.Groups[6].Value == "azalmış") ykkeAnnual = -ykkeAnnual;

                // 2. Bölge grafiklerini oku
                var lines = bulletinText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

                var priceStart = FindSectionStart(lines, "lgelere Göre Konut Fiyat Endeksi");
                if (priceStart < 0)
                    return null;

                var rentStart = FindSectionStart(lines, "lgelere Göre Yeni Kiracı Kira Endeksi");
                if (rentStart < 0)
                    return null;

                var regionalPriceChanges = ParseSection(lines, priceStart);
                var regionalRentChanges = ParseSection(lines, rentStart);

                // 3. Doğrulama
                if (regionalPriceChanges.Count < FibiMarketDataSyncConstants.MinimumRegionalChangeCount ||
                    regionalRentChanges.Count < FibiMarketDataSyncConstants.MinimumRegionalChangeCount)
                {
                    return null;
                }

                var maxPct = FibiMarketDataSyncConstants.MaximumAbsoluteChangePercentage;

                if (Math.Abs(kfeMonthly) > maxPct ||
                    Math.Abs(kfeAnnual) > maxPct ||
                    Math.Abs(ykkeMonthly) > maxPct ||
                    Math.Abs(ykkeAnnual) > maxPct)
                {
                    return null;
                }

                if (regionalPriceChanges.Any(x => Math.Abs(x.AnnualChange) > maxPct || x.ProvinceKeys.Count == 0) ||
                    regionalRentChanges.Any(x => Math.Abs(x.AnnualChange) > maxPct || x.ProvinceKeys.Count == 0))
                {
                    return null;
                }

                return new KfeBulletinData(
                    period,
                    kfeMonthly,
                    kfeAnnual,
                    ykkeMonthly,
                    ykkeAnnual,
                    regionalPriceChanges,
                    regionalRentChanges);
            }
            catch
            {
                return null;
            }
        }

        private static int FindSectionStart(List<string> lines, string keyword)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        private static List<KfeRegionalChange> ParseSection(List<string> lines, int startIndex)
        {
            var result = new List<KfeRegionalChange>();
            string? currentLabel = null;
            decimal? currentValue = null;

            void FlushCurrent()
            {
                if (!string.IsNullOrWhiteSpace(currentLabel) && currentValue.HasValue)
                {
                    var keys = currentLabel.Split(',')
                        .Select(TurkishTextHelper.ToComparisonKey)
                        .Where(k => !string.IsNullOrEmpty(k))
                        .ToList();

                    if (keys.Count > 0 && !(keys.Count == 1 && keys[0] == "TÜRKİYE"))
                    {
                        result.Add(new KfeRegionalChange(keys, currentValue.Value));
                    }
                }
                currentLabel = null;
                currentValue = null;
            }

            void HandleLabel(string labelPart, decimal? value)
            {
                if (currentLabel != null && currentLabel.TrimEnd().EndsWith(","))
                {
                    currentLabel = currentLabel + " " + labelPart;
                    if (value.HasValue)
                    {
                        currentValue = value;
                    }
                }
                else
                {
                    FlushCurrent();
                    currentLabel = labelPart;
                    currentValue = value;
                }
            }

            for (var i = startIndex; i < lines.Count; i++)
            {
                var trimmed = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                if (IsSectionEnd(trimmed))
                    break;

                // İçinde 2 veya daha fazla sayı olan satırlar (eksen) atlanır
                var numberMatches = Regex.Matches(trimmed, @"-?\d+(?:,\d+)?");
                if (numberMatches.Count >= 2)
                    continue;

                // Yalnızca sayı
                if (Regex.IsMatch(trimmed, @"^-?\d{1,3},\d+$"))
                {
                    if (TryParseDecimal(trimmed, out var val))
                    {
                        currentValue = val;
                    }
                    continue;
                }

                // <etiket> <sayı>
                var matchBoth = Regex.Match(trimmed, @"^(?<label>.+?)\s+(?<val>-?\d{1,3},\d+)$");
                if (matchBoth.Success)
                {
                    var labelPart = matchBoth.Groups["label"].Value.Trim();
                    if (TryParseDecimal(matchBoth.Groups["val"].Value, out var val))
                    {
                        HandleLabel(labelPart, val);
                    }
                    continue;
                }

                // Yalnızca etiket
                HandleLabel(trimmed, null);
            }

            FlushCurrent();
            return result;
        }

        private static bool IsSectionEnd(string trimmedLine)
        {
            return trimmedLine.StartsWith("=====") ||
                   trimmedLine.StartsWith("Grafik", StringComparison.OrdinalIgnoreCase) ||
                   trimmedLine.StartsWith("Konut Fiyat Endeksi Resmi", StringComparison.OrdinalIgnoreCase) ||
                   trimmedLine.StartsWith("Yeni Kiracı Kira Endeksi Resmi", StringComparison.OrdinalIgnoreCase) ||
                   trimmedLine.StartsWith("Konut Fiyat Endeksi ve Yeni Kiracı", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryParseDecimal(string? text, out decimal val)
        {
            val = 0m;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var cleaned = text.Trim().Replace(',', '.');
            return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out val);
        }

        private static int? ParseTurkishMonth(string monthName)
        {
            var key = TurkishTextHelper.ToComparisonKey(monthName);
            return key switch
            {
                "OCAK" => 1,
                "SUBAT" or "ŞUBAT" => 2,
                "MART" => 3,
                "NİSAN" or "NISAN" => 4,
                "MAYIS" => 5,
                "HAZİRAN" or "HAZIRAN" => 6,
                "TEMMUZ" => 7,
                "AĞUSTOS" or "AGUSTOS" => 8,
                "EYLÜL" or "EYLUL" => 9,
                "EKİM" or "EKIM" => 10,
                "KASIM" => 11,
                "ARALIK" => 12,
                _ => null
            };
        }
    }
}
