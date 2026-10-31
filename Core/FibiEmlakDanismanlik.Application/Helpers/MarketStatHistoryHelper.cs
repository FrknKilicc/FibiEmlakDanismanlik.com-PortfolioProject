using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>Emsal geçmiş çeyrek fiyatları JSON yardımcı sınıfı.</summary>
    public static class MarketStatHistoryHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static decimal RoundToHundreds(decimal value) =>
            Math.Round(value / 100m, MidpointRounding.AwayFromZero) * 100m;

        public static string ToQuarterLabel(string period)
        {
            if (string.IsNullOrWhiteSpace(period))
                return string.Empty;

            var parts = period.Trim().Split('-');
            if (parts.Length >= 2 && int.TryParse(parts[0], out var year) && int.TryParse(parts[1], out var month) && month >= 1 && month <= 12)
            {
                var quarter = ((month - 1) / 3) + 1;
                return $"{year}-Q{quarter}";
            }

            return period;
        }

        public static string GenerateSeedHistory(decimal currentPrice, decimal annualGrowthRate, string endPeriod, int quarterCount)
        {
            if (quarterCount <= 0)
                return "[]";

            var endQuarterLabel = ToQuarterLabel(endPeriod);
            var parsed = TryParseQuarter(endQuarterLabel);
            int endYear = parsed?.Year ?? DateTime.Today.Year;
            int endQuarter = parsed?.Quarter ?? (((DateTime.Today.Month - 1) / 3) + 1);

            var rateDouble = (double)annualGrowthRate;
            var baseVal = Math.Max(0.0001, 1.0 + (rateDouble / 100.0));
            var quarterlyFactor = (decimal)Math.Pow(baseVal, 0.25);

            var items = new FibiQuarterlyPriceResult[quarterCount];
            var price = currentPrice;

            var curYear = endYear;
            var curQuarter = endQuarter;

            for (var i = quarterCount - 1; i >= 0; i--)
            {
                var label = $"{curYear}-Q{curQuarter}";
                items[i] = new FibiQuarterlyPriceResult(label, RoundToHundreds(price));

                if (quarterlyFactor > 0m)
                {
                    price /= quarterlyFactor;
                }

                curQuarter--;
                if (curQuarter < 1)
                {
                    curQuarter = 4;
                    curYear--;
                }
            }

            return JsonSerializer.Serialize(items, JsonOptions);
        }

        public static string UpsertQuarter(string? historyJson, string quarterLabel, decimal price, int maxCount)
        {
            var list = ReadHistory(historyJson);
            var rounded = RoundToHundreds(price);

            if (list.Count > 0 && string.Equals(list[^1].Quarter, quarterLabel, StringComparison.OrdinalIgnoreCase))
            {
                list[^1] = new FibiQuarterlyPriceResult(quarterLabel, rounded);
            }
            else
            {
                list.Add(new FibiQuarterlyPriceResult(quarterLabel, rounded));
            }

            if (maxCount > 0 && list.Count > maxCount)
            {
                list = list.Skip(list.Count - maxCount).ToList();
            }

            return JsonSerializer.Serialize(list, JsonOptions);
        }

        public static List<FibiQuarterlyPriceResult> ReadHistory(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<FibiQuarterlyPriceResult>();

            try
            {
                var rawList = JsonSerializer.Deserialize<List<FibiQuarterlyPriceResult>>(json, JsonOptions);
                if (rawList == null || rawList.Count == 0)
                    return new List<FibiQuarterlyPriceResult>();

                var result = new List<FibiQuarterlyPriceResult>(rawList.Count);
                foreach (var item in rawList)
                {
                    if (item != null && !string.IsNullOrWhiteSpace(item.Quarter))
                    {
                        result.Add(new FibiQuarterlyPriceResult(item.Quarter, item.Price));
                    }
                }
                return result;
            }
            catch (JsonException)
            {
                return new List<FibiQuarterlyPriceResult>();
            }
        }

        private static (int Year, int Quarter)? TryParseQuarter(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var parts = input.Trim().Split(new[] { '-', ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out var year))
            {
                var qPart = parts[1].Trim().ToUpperInvariant();
                if (qPart.StartsWith("Q") && int.TryParse(qPart.Substring(1), out var q) && q >= 1 && q <= 4)
                {
                    return (year, q);
                }
            }

            return null;
        }
    }
}
