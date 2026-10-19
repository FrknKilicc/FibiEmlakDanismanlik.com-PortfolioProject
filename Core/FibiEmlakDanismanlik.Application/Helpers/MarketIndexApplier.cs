using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>Resmi endeks/birim değerleri emsal kayıtlarına uygulayan saf hesaplama.</summary>
    public static class MarketIndexApplier
    {
        public static bool ApplyBulletin(ExternalMarketStat stat, string? provinceKey, KfeBulletinData data, DateTime now)
        {
            if (!string.IsNullOrWhiteSpace(stat.LastAppliedIndexPeriod) &&
                string.CompareOrdinal(data.Period, stat.LastAppliedIndexPeriod) <= 0)
            {
                return false;
            }

            var monthsDiff = 1;
            if (!string.IsNullOrWhiteSpace(stat.LastAppliedIndexPeriod))
            {
                var lastParts = stat.LastAppliedIndexPeriod.Split('-');
                var curParts = data.Period.Split('-');
                if (lastParts.Length == 2 && curParts.Length == 2 &&
                    int.TryParse(lastParts[0], out var ly) && int.TryParse(lastParts[1], out var lm) &&
                    int.TryParse(curParts[0], out var cy) && int.TryParse(curParts[1], out var cm))
                {
                    var diff = ((cy - ly) * 12) + (cm - lm);
                    monthsDiff = Math.Clamp(diff, 1, 12);
                }
            }

            var normKey = TurkishTextHelper.ToComparisonKey(provinceKey);

            var annualPriceChange = data.NationalAnnualPriceChange;
            if (!string.IsNullOrEmpty(normKey))
            {
                var regionalPrice = data.RegionalPriceChanges.FirstOrDefault(r => r.ProvinceKeys.Contains(normKey));
                if (regionalPrice != null)
                {
                    annualPriceChange = regionalPrice.AnnualChange;
                }
            }

            var annualRentChange = data.NationalAnnualRentChange;
            if (!string.IsNullOrEmpty(normKey))
            {
                var regionalRent = data.RegionalRentChanges.FirstOrDefault(r => r.ProvinceKeys.Contains(normKey));
                if (regionalRent != null)
                {
                    annualRentChange = regionalRent.AnnualChange;
                }
            }

            var priceFactor = Math.Pow(Math.Max(0.0, 1.0 + ((double)annualPriceChange / 100.0)), (double)monthsDiff / 12.0);
            stat.BaseSalePricePerSqm = Math.Round(stat.BaseSalePricePerSqm * (decimal)priceFactor, 0, MidpointRounding.AwayFromZero);

            var rentFactor = Math.Pow(Math.Max(0.0, 1.0 + ((double)annualRentChange / 100.0)), (double)monthsDiff / 12.0);
            stat.BaseRentPricePerSqm = Math.Round(stat.BaseRentPricePerSqm * (decimal)rentFactor, 0, MidpointRounding.AwayFromZero);

            stat.Last1YearGrowthRate = annualPriceChange;

            stat.HistoricalQuarterlyPricesJson = MarketStatHistoryHelper.UpsertQuarter(
                stat.HistoricalQuarterlyPricesJson,
                MarketStatHistoryHelper.ToQuarterLabel(data.Period),
                stat.BaseSalePricePerSqm,
                FibiMarketDataSyncConstants.HistoryQuarterCount);

            stat.LastAppliedIndexPeriod = data.Period;
            stat.LastUpdatedDate = now;

            if (stat.DataSource == FibiServiceConstants.EstimatedSeedDataSource)
            {
                stat.DataSource = FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource;
            }

            return true;
        }

        public static bool ApplyCityUnitValue(ExternalMarketStat cityOrCountryStat, IReadOnlyList<ExternalMarketStat> childStats, CityUnitValue value, DateTime now)
        {
            if (value.SalePricePerSqm <= 0m || value.RentPricePerSqm <= 0m)
                return false;

            var oldSalePrice = cityOrCountryStat.BaseSalePricePerSqm;
            var oldRentPrice = cityOrCountryStat.BaseRentPricePerSqm;

            var saleRatio = oldSalePrice > 0m ? (value.SalePricePerSqm / oldSalePrice) : 1m;
            var rentRatio = oldRentPrice > 0m ? (value.RentPricePerSqm / oldRentPrice) : 1m;

            var quarterLabel = MarketStatHistoryHelper.ToQuarterLabel(value.Period);

            cityOrCountryStat.BaseSalePricePerSqm = Math.Round(value.SalePricePerSqm, 0, MidpointRounding.AwayFromZero);
            cityOrCountryStat.BaseRentPricePerSqm = Math.Round(value.RentPricePerSqm, 0, MidpointRounding.AwayFromZero);
            cityOrCountryStat.LastAppliedIndexPeriod = value.Period;
            cityOrCountryStat.LastUpdatedDate = now;
            cityOrCountryStat.HistoricalQuarterlyPricesJson = MarketStatHistoryHelper.UpsertQuarter(
                cityOrCountryStat.HistoricalQuarterlyPricesJson,
                quarterLabel,
                cityOrCountryStat.BaseSalePricePerSqm,
                FibiMarketDataSyncConstants.HistoryQuarterCount);
            cityOrCountryStat.DataSource = FibiMarketDataSyncConstants.EvdsDataSource;

            foreach (var child in childStats)
            {
                if (oldSalePrice > 0m)
                {
                    child.BaseSalePricePerSqm = Math.Round(child.BaseSalePricePerSqm * saleRatio, 0, MidpointRounding.AwayFromZero);
                }
                if (oldRentPrice > 0m)
                {
                    child.BaseRentPricePerSqm = Math.Round(child.BaseRentPricePerSqm * rentRatio, 0, MidpointRounding.AwayFromZero);
                }

                child.LastAppliedIndexPeriod = value.Period;
                child.LastUpdatedDate = now;
                child.HistoricalQuarterlyPricesJson = MarketStatHistoryHelper.UpsertQuarter(
                    child.HistoricalQuarterlyPricesJson,
                    quarterLabel,
                    child.BaseSalePricePerSqm,
                    FibiMarketDataSyncConstants.HistoryQuarterCount);

                if (child.DataSource == FibiServiceConstants.EstimatedSeedDataSource)
                {
                    child.DataSource = FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource;
                }
            }

            return true;
        }

        public static bool IsEstimatedSource(string? dataSource)
        {
            return dataSource == FibiServiceConstants.EstimatedSeedDataSource ||
                   dataSource == FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource;
        }
    }
}
