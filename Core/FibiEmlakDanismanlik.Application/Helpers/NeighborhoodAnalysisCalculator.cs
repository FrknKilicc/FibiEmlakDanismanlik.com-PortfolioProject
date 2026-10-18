using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>FİBİ bölge analizi saf hesaplama motoru (veritabanı bağımsız).</summary>
    public static class NeighborhoodAnalysisCalculator
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static decimal GetSegmentCoefficient(PropertySegment segment) => segment switch
        {
            PropertySegment.StandardApartment => FibiNeighborhoodAnalysisConstants.StandardApartmentCoefficient,
            PropertySegment.NewOrGatedCommunity => FibiNeighborhoodAnalysisConstants.NewOrGatedCommunityCoefficient,
            PropertySegment.Commercial => FibiNeighborhoodAnalysisConstants.CommercialCoefficient,
            _ => FibiNeighborhoodAnalysisConstants.StandardApartmentCoefficient
        };

        public static NeighborhoodAnalysisItemResult Calculate(ExternalMarketStat stat, PropertySegment segment)
        {
            var coefficient = GetSegmentCoefficient(segment);

            // 1. Fiyat bantları (100'e yuvarlanır)
            var balanced = RoundToHundreds(stat.BaseSalePricePerSqm * coefficient);
            var min = RoundToHundreds(balanced * (1m - (stat.BandSpreadPercentage / 100m)));
            var premium = RoundToHundreds(balanced * stat.AgencyMarginMultiplier);
            var priceBand = new FibiPriceBandResult(min, balanced, premium);

            // 2. Amortisman ve kira verimi
            var monthlyRent = stat.BaseRentPricePerSqm * coefficient;
            decimal amortizationYears;
            decimal annualRentYield;
            string amortizationVerdict;

            if (monthlyRent <= 0m)
            {
                amortizationYears = 0m;
                annualRentYield = 0m;
                amortizationVerdict = FibiNeighborhoodAnalysisConstants.ValueGrowthVerdict;
            }
            else
            {
                var rawYears = balanced / (monthlyRent * 12m);
                amortizationYears = Math.Round(rawYears, 1, MidpointRounding.AwayFromZero);
                annualRentYield = amortizationYears > 0m
                    ? Math.Round(100m / amortizationYears, 2, MidpointRounding.AwayFromZero)
                    : 0m;

                if (amortizationYears < FibiNeighborhoodAnalysisConstants.HighRentYieldMaxYears)
                    amortizationVerdict = FibiNeighborhoodAnalysisConstants.HighRentYieldVerdict;
                else if (amortizationYears <= FibiNeighborhoodAnalysisConstants.BalancedMaxYears)
                    amortizationVerdict = FibiNeighborhoodAnalysisConstants.BalancedVerdict;
                else
                    amortizationVerdict = FibiNeighborhoodAnalysisConstants.ValueGrowthVerdict;
            }

            // 3. Fiyat geçmişi
            var historicalPrices = ParseHistoricalPrices(stat.HistoricalQuarterlyPricesJson, coefficient);

            // 4. Büyüme oranları ve projeksiyon
            var balancedRate = (stat.Last1YearGrowthRate * FibiNeighborhoodAnalysisConstants.GrowthRateWeight) +
                               (stat.RegionalDevelopmentFactor * FibiNeighborhoodAnalysisConstants.DevelopmentFactorWeight);
            var conservativeRate = balancedRate * FibiNeighborhoodAnalysisConstants.ConservativeMultiplier;
            var optimisticRate = balancedRate * FibiNeighborhoodAnalysisConstants.OptimisticMultiplier;

            var expected12MonthGrowthRate = Math.Round(balancedRate, 1, MidpointRounding.AwayFromZero);
            var conservative12MonthGrowthRate = Math.Round(conservativeRate, 1, MidpointRounding.AwayFromZero);
            var optimistic12MonthGrowthRate = Math.Round(optimisticRate, 1, MidpointRounding.AwayFromZero);

            var startPrice = historicalPrices.Count > 0
                ? historicalPrices[^1].Price
                : balanced;

            var lastQuarterLabel = historicalPrices.Count > 0 ? historicalPrices[^1].Quarter : null;
            var quarterLabels = GenerateProjectionQuarters(lastQuarterLabel, FibiNeighborhoodAnalysisConstants.ProjectionQuarterCount);

            var consFactor = CalculateQuarterlyFactor(conservativeRate);
            var balFactor = CalculateQuarterlyFactor(balancedRate);
            var optFactor = CalculateQuarterlyFactor(optimisticRate);

            var projection = new List<FibiProjectionPointResult>(FibiNeighborhoodAnalysisConstants.ProjectionQuarterCount);
            var currentCons = startPrice;
            var currentBal = startPrice;
            var currentOpt = startPrice;

            for (var i = 0; i < FibiNeighborhoodAnalysisConstants.ProjectionQuarterCount; i++)
            {
                currentCons *= consFactor;
                currentBal *= balFactor;
                currentOpt *= optFactor;

                projection.Add(new FibiProjectionPointResult(
                    quarterLabels[i],
                    RoundToHundreds(currentCons),
                    RoundToHundreds(currentBal),
                    RoundToHundreds(currentOpt)));
            }

            // 5. Yatırım notu
            decimal amortizationScore;
            if (amortizationYears <= 0m)
            {
                amortizationScore = 0m;
            }
            else if (amortizationYears <= FibiNeighborhoodAnalysisConstants.BestAmortizationYears)
            {
                amortizationScore = 100m;
            }
            else if (amortizationYears >= FibiNeighborhoodAnalysisConstants.WorstAmortizationYears)
            {
                amortizationScore = 0m;
            }
            else
            {
                var range = FibiNeighborhoodAnalysisConstants.WorstAmortizationYears - FibiNeighborhoodAnalysisConstants.BestAmortizationYears;
                amortizationScore = ((FibiNeighborhoodAnalysisConstants.WorstAmortizationYears - amortizationYears) / range) * 100m;
            }

            var growthRatio = (stat.Last1YearGrowthRate / FibiNeighborhoodAnalysisConstants.MaxGrowthRateForFullScore) * 100m;
            var growthScore = Math.Clamp(growthRatio, 0m, 100m);
            var appetite = (decimal)stat.UrbanTransformationAppetiteScore;

            var rawInvestmentScore = (amortizationScore * FibiNeighborhoodAnalysisConstants.AmortizationWeight) +
                                     (growthScore * FibiNeighborhoodAnalysisConstants.GrowthWeight) +
                                     (appetite * FibiNeighborhoodAnalysisConstants.AppetiteWeight);

            var investmentScore = Math.Clamp((int)Math.Round(rawInvestmentScore, MidpointRounding.AwayFromZero), 0, 100);

            var investmentGrade = investmentScore >= FibiNeighborhoodAnalysisConstants.GradeAPlusThreshold
                ? "A+"
                : investmentScore >= FibiNeighborhoodAnalysisConstants.GradeAThreshold
                    ? "A"
                    : investmentScore >= FibiNeighborhoodAnalysisConstants.GradeBThreshold
                        ? "B"
                        : "C";

            return new NeighborhoodAnalysisItemResult
            {
                Location = FibiLocationSummaryFactory.FromNeighborhood(stat.Neighborhood!),
                PriceBand = priceBand,
                AmortizationYears = amortizationYears,
                AnnualRentYieldPercentage = annualRentYield,
                AmortizationVerdict = amortizationVerdict,
                Expected12MonthGrowthRate = expected12MonthGrowthRate,
                Conservative12MonthGrowthRate = conservative12MonthGrowthRate,
                Optimistic12MonthGrowthRate = optimistic12MonthGrowthRate,
                Last1YearGrowthRate = stat.Last1YearGrowthRate,
                Last3YearsCompoundGrowthRate = stat.Last3YearsCompoundGrowthRate,
                InvestmentScore = investmentScore,
                InvestmentGrade = investmentGrade,
                HistoricalPrices = historicalPrices,
                Projection = projection
            };
        }

        private static decimal RoundToHundreds(decimal value) =>
            Math.Round(value / 100m, MidpointRounding.AwayFromZero) * 100m;

        private static decimal CalculateQuarterlyFactor(decimal annualRate)
        {
            var rateDouble = (double)annualRate;
            var baseVal = Math.Max(0.0, 1.0 + (rateDouble / 100.0));
            return (decimal)Math.Pow(baseVal, 0.25);
        }

        private static List<FibiQuarterlyPriceResult> ParseHistoricalPrices(string? json, decimal coefficient)
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
                        result.Add(new FibiQuarterlyPriceResult(
                            item.Quarter,
                            RoundToHundreds(item.Price * coefficient)));
                    }
                }

                return result;
            }
            catch (JsonException)
            {
                return new List<FibiQuarterlyPriceResult>();
            }
        }

        private static List<string> GenerateProjectionQuarters(string? lastHistoricalQuarter, int count)
        {
            var parsed = TryParseQuarter(lastHistoricalQuarter);
            int year;
            int quarter;

            if (parsed.HasValue)
            {
                year = parsed.Value.Year;
                quarter = parsed.Value.Quarter + 1;
                if (quarter > 4)
                {
                    year++;
                    quarter = 1;
                }
            }
            else
            {
                var today = DateTime.Today;
                year = today.Year;
                quarter = ((today.Month - 1) / 3) + 1;
            }

            var list = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                list.Add($"{year}-Q{quarter}");
                quarter++;
                if (quarter > 4)
                {
                    year++;
                    quarter = 1;
                }
            }

            return list;
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
