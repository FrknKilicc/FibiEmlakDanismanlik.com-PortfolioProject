using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using Xunit;

namespace FibiEmlakDanismanlik.Unit.Tests
{
    public class MarketIndexApplierTests
    {
        [Fact]
        public void ApplyBulletin_ValidNewerPeriod_UpdatesPricesAndReturnsTrue()
        {
            // Yeni dönem bülteni uygulandığında emsal fiyatları güncellenmeli ve kaynak etiketi değişmeli
            var stat = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 50000m,
                BaseRentPricePerSqm = 250m,
                LastAppliedIndexPeriod = "2024-01",
                DataSource = FibiServiceConstants.EstimatedSeedDataSource,
                HistoricalQuarterlyPricesJson = "[]"
            };

            var data = new KfeBulletinData(
                Period: "2024-02",
                NationalMonthlyPriceChange: 2m,
                NationalAnnualPriceChange: 40m,
                NationalMonthlyRentChange: 3m,
                NationalAnnualRentChange: 50m,
                RegionalPriceChanges: new List<KfeRegionalChange>
                {
                    new(new[] { "İSTANBUL" }, 44m)
                },
                RegionalRentChanges: new List<KfeRegionalChange>
                {
                    new(new[] { "İSTANBUL" }, 60m)
                });

            var now = new DateTime(2024, 3, 1, 10, 0, 0, DateTimeKind.Utc);

            var applied = MarketIndexApplier.ApplyBulletin(stat, "İstanbul", data, now);

            Assert.True(applied);
            Assert.True(stat.BaseSalePricePerSqm > 50000m);
            Assert.True(stat.BaseRentPricePerSqm > 250m);
            Assert.Equal(44m, stat.Last1YearGrowthRate);
            Assert.Equal("2024-02", stat.LastAppliedIndexPeriod);
            Assert.Equal(now, stat.LastUpdatedDate);
            Assert.Equal(FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource, stat.DataSource);
            Assert.Contains("2024-Q1", stat.HistoricalQuarterlyPricesJson);
        }

        [Theory]
        [InlineData("2024-05", "2024-05")] // Aynı dönem
        [InlineData("2024-05", "2024-04")] // Daha eski dönem
        public void ApplyBulletin_OlderOrSamePeriod_ReturnsFalseAndLeavesStatUnchanged(string currentPeriod, string bulletinPeriod)
        {
            // Zaten uygulanmış veya daha eski bir bülten tekrar uygulanmamalı
            var stat = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 50000m,
                BaseRentPricePerSqm = 250m,
                LastAppliedIndexPeriod = currentPeriod,
                DataSource = FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource,
                HistoricalQuarterlyPricesJson = "[]"
            };

            var data = new KfeBulletinData(
                Period: bulletinPeriod,
                NationalMonthlyPriceChange: 2m,
                NationalAnnualPriceChange: 40m,
                NationalMonthlyRentChange: 3m,
                NationalAnnualRentChange: 50m,
                RegionalPriceChanges: new List<KfeRegionalChange>(),
                RegionalRentChanges: new List<KfeRegionalChange>());

            var applied = MarketIndexApplier.ApplyBulletin(stat, "Ankara", data, DateTime.UtcNow);

            Assert.False(applied);
            Assert.Equal(50000m, stat.BaseSalePricePerSqm);
            Assert.Equal(250m, stat.BaseRentPricePerSqm);
            Assert.Equal(currentPeriod, stat.LastAppliedIndexPeriod);
        }

        [Fact]
        public void ApplyBulletin_NoRegionalMatch_FallsBackToNationalChanges()
        {
            // İl bölge listesinde bulunamazsa ulusal yıllık değişim oranları uygulanmalı
            var stat = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 40000m,
                BaseRentPricePerSqm = 200m,
                LastAppliedIndexPeriod = "2024-01",
                HistoricalQuarterlyPricesJson = "[]"
            };

            var data = new KfeBulletinData(
                Period: "2024-02",
                NationalMonthlyPriceChange: 2m,
                NationalAnnualPriceChange: 35m,
                NationalMonthlyRentChange: 3m,
                NationalAnnualRentChange: 45m,
                RegionalPriceChanges: new List<KfeRegionalChange>
                {
                    new(new[] { "İSTANBUL" }, 50m)
                },
                RegionalRentChanges: new List<KfeRegionalChange>
                {
                    new(new[] { "İSTANBUL" }, 60m)
                });

            var applied = MarketIndexApplier.ApplyBulletin(stat, "BilinmeyenIl", data, DateTime.UtcNow);

            Assert.True(applied);
            Assert.Equal(35m, stat.Last1YearGrowthRate);
        }

        [Fact]
        public void ApplyCityUnitValue_ValidValues_UpdatesParentAndChildStatsProportionally()
        {
            // İl birim m² değeri uygulandığında ana kayıt güncellenmeli, alt kayıtlar oranlanarak güncellenmeli
            var parent = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 50000m,
                BaseRentPricePerSqm = 250m,
                DataSource = FibiServiceConstants.EstimatedSeedDataSource,
                HistoricalQuarterlyPricesJson = "[]"
            };

            var child = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 60000m,
                BaseRentPricePerSqm = 300m,
                DataSource = FibiServiceConstants.EstimatedSeedDataSource,
                HistoricalQuarterlyPricesJson = "[]"
            };

            var value = new CityUnitValue(
                CityPlaceCode: 34,
                SalePricePerSqm: 60000m, // 60000 / 50000 = 1.2 kat
                RentPricePerSqm: 300m,   // 300 / 250 = 1.2 kat
                Period: "2024-03");

            var now = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc);

            var applied = MarketIndexApplier.ApplyCityUnitValue(parent, new[] { child }, value, now);

            Assert.True(applied);
            Assert.Equal(60000m, parent.BaseSalePricePerSqm);
            Assert.Equal(300m, parent.BaseRentPricePerSqm);
            Assert.Equal(FibiMarketDataSyncConstants.EvdsDataSource, parent.DataSource);
            Assert.Equal("2024-03", parent.LastAppliedIndexPeriod);
            Assert.Equal(now, parent.LastUpdatedDate);

            // Alt kayıt (ilçe/mahalle) 1.2 çarpanı ile güncellenmiş olmalı
            Assert.Equal(72000m, child.BaseSalePricePerSqm);
            Assert.Equal(360m, child.BaseRentPricePerSqm);
            Assert.Equal(FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource, child.DataSource);
            Assert.Equal("2024-03", child.LastAppliedIndexPeriod);
        }

        [Theory]
        [InlineData(0, 300)]
        [InlineData(-100, 300)]
        [InlineData(50000, 0)]
        [InlineData(50000, -50)]
        public void ApplyCityUnitValue_ZeroOrNegativeValues_ReturnsFalse(decimal salePrice, decimal rentPrice)
        {
            // Satış veya kira birim fiyatı sıfır ya da negatif ise işlem reddedilmeli
            var parent = new ExternalMarketStat
            {
                BaseSalePricePerSqm = 50000m,
                BaseRentPricePerSqm = 250m,
                HistoricalQuarterlyPricesJson = "[]"
            };

            var value = new CityUnitValue(34, salePrice, rentPrice, "2024-03");

            var applied = MarketIndexApplier.ApplyCityUnitValue(parent, Array.Empty<ExternalMarketStat>(), value, DateTime.UtcNow);

            Assert.False(applied);
            Assert.Equal(50000m, parent.BaseSalePricePerSqm);
            Assert.Equal(250m, parent.BaseRentPricePerSqm);
        }

        [Theory]
        [InlineData(FibiServiceConstants.EstimatedSeedDataSource, true)]
        [InlineData(FibiMarketDataSyncConstants.IndexAdjustedSeedDataSource, true)]
        [InlineData(FibiMarketDataSyncConstants.EvdsDataSource, false)]
        [InlineData(FibiMarketDataSyncConstants.IbbSeismicDataSource, false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void IsEstimatedSource_VariousDataSources_ReturnsExpectedBoolean(string? dataSource, bool expected)
        {
            // Yalnızca tohum veya tohumdan endekslenmiş kaynaklar tahmini kabul edilmelidir
            var result = MarketIndexApplier.IsEstimatedSource(dataSource);

            Assert.Equal(expected, result);
        }
    }
}
