using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using System;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>Mahalle deprem tehlikesi ve bina hasar senaryosu çözümleme motoru.</summary>
    public static class SeismicHazardResolver
    {
        public static SeismicHazardAssessment Resolve(NeighborhoodSeismicScenarioStat? scenario, ExternalMarketStat stat)
        {
            if (scenario != null)
            {
                var totalBuildings = scenario.BuildingsBefore1980Count + scenario.Buildings1980To2000Count + scenario.BuildingsAfter2000Count;
                if (totalBuildings > 0)
                {
                    var heavyDamageCount = scenario.VeryHeavilyDamagedBuildingCount + scenario.HeavilyDamagedBuildingCount;
                    var damageRate = Math.Round((decimal)heavyDamageCount / totalBuildings * 100m, 1, MidpointRounding.AwayFromZero);

                    SoilRiskClass level;
                    if (damageRate >= FibiMarketDataConstants.SeismicVeryHighDamageRateThreshold)
                        level = SoilRiskClass.VeryHigh;
                    else if (damageRate >= FibiMarketDataConstants.SeismicHighDamageRateThreshold)
                        level = SoilRiskClass.High;
                    else if (damageRate >= FibiMarketDataConstants.SeismicMediumDamageRateThreshold)
                        level = SoilRiskClass.Medium;
                    else
                        level = SoilRiskClass.Low;

                    var pre1980Share = Math.Round((decimal)scenario.BuildingsBefore1980Count / totalBuildings * 100m, 1, MidpointRounding.AwayFromZero);

                    var summary = new FibiSeismicSummaryResult(
                        scenario.DataSource,
                        true,
                        GetHazardLabel(level),
                        damageRate,
                        totalBuildings,
                        pre1980Share);

                    return new SeismicHazardAssessment(level, summary);
                }
            }

            if (stat.SoilRiskClass.HasValue)
            {
                var level = stat.SoilRiskClass.Value;
                var sourceName = string.Format(
                    FibiMarketDataConstants.RegionalSoilEstimateSourceFormat,
                    FibiDataSourceFactory.GetScopeLabel(stat.Scope));

                var summary = new FibiSeismicSummaryResult(
                    sourceName,
                    false,
                    GetHazardLabel(level),
                    null,
                    null,
                    null);

                return new SeismicHazardAssessment(level, summary);
            }

            var unknownSummary = new FibiSeismicSummaryResult(
                FibiMarketDataConstants.NoSeismicDataSource,
                false,
                GetHazardLabel(null),
                null,
                null,
                null);

            return new SeismicHazardAssessment(null, unknownSummary);
        }

        public static string GetHazardLabel(SoilRiskClass? level) => level switch
        {
            SoilRiskClass.Low => FibiMarketDataConstants.HazardLowLabel,
            SoilRiskClass.Medium => FibiMarketDataConstants.HazardMediumLabel,
            SoilRiskClass.High => FibiMarketDataConstants.HazardHighLabel,
            SoilRiskClass.VeryHigh => FibiMarketDataConstants.HazardVeryHighLabel,
            _ => FibiMarketDataConstants.HazardUnknownLabel
        };
    }
}
