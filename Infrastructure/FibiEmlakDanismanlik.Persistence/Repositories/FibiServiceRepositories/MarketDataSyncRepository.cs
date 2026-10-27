using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.FibiServiceRepositories
{
    public class MarketDataSyncRepository : IMarketDataSyncRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public MarketDataSyncRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<DateTime?> GetLastSuccessfulSyncAsync(string providerKey)
        {
            return await _context.MarketDataSyncLogs
                .Where(x => x.ProviderKey == providerKey && x.IsSuccess)
                .OrderByDescending(x => x.FinishedAt)
                .Select(x => (DateTime?)x.FinishedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddSyncLogAsync(MarketDataSyncLog log)
        {
            await _context.MarketDataSyncLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<int> InsertMissingSeedStatsAsync(IReadOnlyList<MarketStatSeedRecord> records, string seedPeriod, DateTime now)
        {
            var cities = await _context.Cities
                .AsNoTracking()
                .Where(c => c.PlaceCode.HasValue)
                .ToListAsync();

            var cityMap = new Dictionary<int, int>();
            foreach (var c in cities)
            {
                if (c.PlaceCode.HasValue)
                {
                    cityMap.TryAdd(c.PlaceCode.Value, c.CityId);
                }
            }

            var districts = await _context.Districts
                .AsNoTracking()
                .ToListAsync();

            var districtMap = new Dictionary<(int CityId, string Key), int>();
            foreach (var d in districts)
            {
                var key = TurkishTextHelper.ToComparisonKey(d.Name);
                districtMap.TryAdd((d.CityId, key), d.DistrictId);
            }

            var existing = await _context.ExternalMarketStats
                .AsNoTracking()
                .Select(x => new { x.Scope, x.CityId, x.DistrictId })
                .ToListAsync();

            var existingKeys = new HashSet<string>();
            foreach (var item in existing)
            {
                existingKeys.Add(BuildScopeKey(item.Scope, item.CityId, item.DistrictId));
            }

            var toAdd = new List<ExternalMarketStat>();

            foreach (var rec in records)
            {
                int? resolvedCityId = null;
                int? resolvedDistrictId = null;

                if (rec.Scope == MarketStatScope.Country)
                {
                    resolvedCityId = null;
                    resolvedDistrictId = null;
                }
                else if (rec.Scope == MarketStatScope.City)
                {
                    if (!rec.CityPlaceCode.HasValue || !cityMap.TryGetValue(rec.CityPlaceCode.Value, out var cityId))
                        continue;

                    resolvedCityId = cityId;
                    resolvedDistrictId = null;
                }
                else if (rec.Scope == MarketStatScope.District)
                {
                    if (!rec.CityPlaceCode.HasValue || string.IsNullOrWhiteSpace(rec.DistrictName))
                        continue;

                    if (!cityMap.TryGetValue(rec.CityPlaceCode.Value, out var cityId))
                        continue;

                    var districtKey = TurkishTextHelper.ToComparisonKey(rec.DistrictName);
                    if (!districtMap.TryGetValue((cityId, districtKey), out var districtId))
                        continue;

                    resolvedCityId = cityId;
                    resolvedDistrictId = districtId;
                }
                else
                {
                    continue;
                }

                var scopeKey = BuildScopeKey(rec.Scope, resolvedCityId, resolvedDistrictId);
                if (!existingKeys.Add(scopeKey))
                    continue;

                var stat = new ExternalMarketStat
                {
                    Scope = rec.Scope,
                    CityId = resolvedCityId,
                    DistrictId = resolvedDistrictId,
                    NeighborhoodId = null,
                    BaseSalePricePerSqm = rec.SalePricePerSqm,
                    BaseRentPricePerSqm = rec.RentPricePerSqm,
                    AgencyMarginMultiplier = FibiServiceConstants.DefaultAgencyMarginMultiplier,
                    BandSpreadPercentage = FibiServiceConstants.DefaultBandSpreadPercentage,
                    Last1YearGrowthRate = rec.Last1YearGrowthRate,
                    Last3YearsCompoundGrowthRate = rec.Last3YearsCompoundGrowthRate,
                    RegionalDevelopmentFactor = rec.RegionalDevelopmentFactor,
                    SoilRiskClass = rec.SoilRiskClass,
                    UrbanTransformationAppetiteScore = rec.UrbanTransformationAppetiteScore,
                    HistoricalQuarterlyPricesJson = MarketStatHistoryHelper.GenerateSeedHistory(
                        rec.SalePricePerSqm,
                        rec.Last1YearGrowthRate,
                        seedPeriod,
                        FibiMarketDataSyncConstants.HistoryQuarterCount),
                    DataSource = FibiServiceConstants.EstimatedSeedDataSource,
                    LastAppliedIndexPeriod = seedPeriod,
                    LastUpdatedDate = now
                };

                toAdd.Add(stat);
            }

            if (toAdd.Count > 0)
            {
                await _context.ExternalMarketStats.AddRangeAsync(toAdd);
                await _context.SaveChangesAsync();
            }

            return toAdd.Count;
        }

        public async Task<bool> HasSeismicScenarioDataAsync()
        {
            return await _context.NeighborhoodSeismicScenarioStats.AnyAsync();
        }

        public async Task<int> UpsertSeismicScenariosAsync(IReadOnlyList<IbbSeismicRecord> records, string dataSource, DateTime now)
        {
            static string GetCompactNeighborhoodKey(string? name)
            {
                var compact = TurkishTextHelper.ToCompactComparisonKey(name);
                if (compact.EndsWith(FibiMarketDataSyncConstants.CompactNeighborhoodSuffix, StringComparison.Ordinal))
                {
                    var trimmed = compact[..^FibiMarketDataSyncConstants.CompactNeighborhoodSuffix.Length];
                    if (trimmed.Length > 0)
                    {
                        return trimmed;
                    }
                }
                else if (compact.EndsWith(FibiMarketDataSyncConstants.CompactNeighborhoodShortSuffix, StringComparison.Ordinal))
                {
                    var trimmed = compact[..^FibiMarketDataSyncConstants.CompactNeighborhoodShortSuffix.Length];
                    if (trimmed.Length > 0)
                    {
                        return trimmed;
                    }
                }

                return compact;
            }

            var istanbulNeighborhoods = await _context.Neighborhoods
                .AsNoTracking()
                .Include(n => n.District)
                    .ThenInclude(d => d.City)
                .Where(n => n.District.City.PlaceCode == FibiMarketDataSyncConstants.IstanbulPlaceCode)
                .ToListAsync();

            var neighborhoodMap = new Dictionary<(string DistrictKey, string NeighborhoodKey), int>();
            var compactNeighborhoodMap = new Dictionary<(string DistrictCompactKey, string NeighborhoodCompactKey), int>();
            var ambiguousCompactKeys = new HashSet<(string DistrictCompactKey, string NeighborhoodCompactKey)>();

            foreach (var n in istanbulNeighborhoods)
            {
                var distKey = TurkishTextHelper.ToComparisonKey(n.District.Name);
                var name = n.Name;
                if (name.EndsWith(FibiMarketDataSyncConstants.NeighborhoodNameSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name[..^FibiMarketDataSyncConstants.NeighborhoodNameSuffix.Length];
                }
                var neighKey = TurkishTextHelper.ToComparisonKey(name);
                neighborhoodMap.TryAdd((distKey, neighKey), n.NeighborhoodId);

                var distCompactKey = TurkishTextHelper.ToCompactComparisonKey(n.District.Name);
                var neighCompactKey = GetCompactNeighborhoodKey(n.Name);
                var compactKey = (distCompactKey, neighCompactKey);

                if (ambiguousCompactKeys.Contains(compactKey))
                {
                    // Zaten belirsiz
                }
                else if (!compactNeighborhoodMap.TryAdd(compactKey, n.NeighborhoodId))
                {
                    compactNeighborhoodMap.Remove(compactKey);
                    ambiguousCompactKeys.Add(compactKey);
                }
            }

            var existingScenarios = await _context.NeighborhoodSeismicScenarioStats
                .ToDictionaryAsync(s => s.NeighborhoodId);

            var affectedCount = 0;

            foreach (var rec in records)
            {
                var recNeighName = rec.NeighborhoodName;
                if (recNeighName.EndsWith(FibiMarketDataSyncConstants.NeighborhoodNameSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    recNeighName = recNeighName[..^FibiMarketDataSyncConstants.NeighborhoodNameSuffix.Length];
                }

                var recDistKey = TurkishTextHelper.ToComparisonKey(rec.DistrictName);
                if (FibiMarketDataSyncConstants.IbbDistrictAliases.TryGetValue(recDistKey, out var aliasDistKey))
                {
                    recDistKey = aliasDistKey;
                }

                var recNeighKey = TurkishTextHelper.ToComparisonKey(recNeighName);

                if (!neighborhoodMap.TryGetValue((recDistKey, recNeighKey), out var neighborhoodId))
                {
                    var recDistCompactKey = TurkishTextHelper.ToCompactComparisonKey(recDistKey);
                    var recNeighCompactKey = GetCompactNeighborhoodKey(rec.NeighborhoodName);

                    if (!compactNeighborhoodMap.TryGetValue((recDistCompactKey, recNeighCompactKey), out neighborhoodId))
                    {
                        continue;
                    }
                }

                if (existingScenarios.TryGetValue(neighborhoodId, out var existing))
                {
                    existing.UavtCode = rec.UavtCode;
                    existing.BuildingsBefore1980Count = rec.BuildingsBefore1980Count;
                    existing.Buildings1980To2000Count = rec.Buildings1980To2000Count;
                    existing.BuildingsAfter2000Count = rec.BuildingsAfter2000Count;
                    existing.VeryHeavilyDamagedBuildingCount = rec.VeryHeavilyDamagedBuildingCount;
                    existing.HeavilyDamagedBuildingCount = rec.HeavilyDamagedBuildingCount;
                    existing.ModeratelyDamagedBuildingCount = rec.ModeratelyDamagedBuildingCount;
                    existing.LightlyDamagedBuildingCount = rec.LightlyDamagedBuildingCount;
                    existing.EstimatedFatalityCount = rec.EstimatedFatalityCount;
                    existing.TemporaryShelterNeedCount = rec.TemporaryShelterNeedCount;
                    existing.DataSource = dataSource;
                    existing.LastUpdatedDate = now;
                    affectedCount++;
                }
                else
                {
                    var newScenario = new NeighborhoodSeismicScenarioStat
                    {
                        NeighborhoodId = neighborhoodId,
                        UavtCode = rec.UavtCode,
                        BuildingsBefore1980Count = rec.BuildingsBefore1980Count,
                        Buildings1980To2000Count = rec.Buildings1980To2000Count,
                        BuildingsAfter2000Count = rec.BuildingsAfter2000Count,
                        VeryHeavilyDamagedBuildingCount = rec.VeryHeavilyDamagedBuildingCount,
                        HeavilyDamagedBuildingCount = rec.HeavilyDamagedBuildingCount,
                        ModeratelyDamagedBuildingCount = rec.ModeratelyDamagedBuildingCount,
                        LightlyDamagedBuildingCount = rec.LightlyDamagedBuildingCount,
                        EstimatedFatalityCount = rec.EstimatedFatalityCount,
                        TemporaryShelterNeedCount = rec.TemporaryShelterNeedCount,
                        DataSource = dataSource,
                        LastUpdatedDate = now
                    };
                    await _context.NeighborhoodSeismicScenarioStats.AddAsync(newScenario);
                    existingScenarios.Add(neighborhoodId, newScenario);
                    affectedCount++;
                }
            }

            if (affectedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return affectedCount;
        }

        public async Task<List<(ExternalMarketStat Stat, int? CityPlaceCode, string? CityName)>> GetTrackedStatsWithCityAsync()
        {
            var items = await _context.ExternalMarketStats
                .Select(x => new
                {
                    Stat = x,
                    CityPlaceCode = x.City != null ? x.City.PlaceCode : null,
                    CityName = x.City != null ? x.City.Name : null
                })
                .ToListAsync();

            return items.Select(x => (x.Stat, x.CityPlaceCode, x.CityName)).ToList();
        }

        public async Task<BankLoanRate?> GetTrackedBankRateAsync(string providerKey)
        {
            return await _context.BankLoanRates.FirstOrDefaultAsync(x => x.ProviderKey == providerKey);
        }

        public async Task<List<MarketDataSyncLog>> GetRecentSyncLogsAsync(int count)
        {
            return await _context.MarketDataSyncLogs
                .AsNoTracking()
                .OrderByDescending(x => x.StartedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void DiscardPendingChanges()
        {
            _context.ChangeTracker.Clear();
        }

        private static string BuildScopeKey(MarketStatScope scope, int? cityId, int? districtId) => scope switch
        {
            MarketStatScope.Country => "Country",
            MarketStatScope.City => $"City:{cityId}",
            MarketStatScope.District => $"District:{cityId}:{districtId}",
            _ => $"Other:{scope}:{cityId}:{districtId}"
        };
    }
}
