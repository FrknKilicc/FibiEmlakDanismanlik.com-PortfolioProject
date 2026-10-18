using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.FibiServiceRepositories
{
    public class MarketStatRepository : IMarketStatRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public MarketStatRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<Neighborhood?> GetNeighborhoodWithParentsAsync(int neighborhoodId)
        {
            return await _context.Neighborhoods
                .AsNoTracking()
                .Include(n => n.District)
                    .ThenInclude(d => d.City)
                .FirstOrDefaultAsync(n => n.NeighborhoodId == neighborhoodId);
        }

        public async Task<ExternalMarketStat?> ResolveStatAsync(int neighborhoodId, int districtId, int cityId)
        {
            return await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => (x.Scope == MarketStatScope.Neighborhood && x.NeighborhoodId == neighborhoodId)
                         || (x.Scope == MarketStatScope.District && x.DistrictId == districtId)
                         || (x.Scope == MarketStatScope.City && x.CityId == cityId)
                         || x.Scope == MarketStatScope.Country)
                .OrderByDescending(x => x.Scope)
                .FirstOrDefaultAsync();
        }

        public async Task<NeighborhoodSeismicScenarioStat?> GetSeismicScenarioAsync(int neighborhoodId)
        {
            return await _context.NeighborhoodSeismicScenarioStats
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NeighborhoodId == neighborhoodId);
        }

        public async Task<List<FibiLocationSummaryResult>> GetCoveredLocationsAsync()
        {
            return await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => x.Scope == MarketStatScope.Neighborhood)
                .OrderBy(x => x.Neighborhood!.District.City.Name)
                .ThenBy(x => x.Neighborhood!.District.Name)
                .ThenBy(x => x.Neighborhood!.Name)
                .Select(x => new FibiLocationSummaryResult(
                    x.Neighborhood!.District.City.CityId,
                    x.Neighborhood!.District.City.Name,
                    x.Neighborhood!.District.DistrictId,
                    x.Neighborhood!.District.Name,
                    x.Neighborhood!.NeighborhoodId,
                    x.Neighborhood!.Name
                ))
                .ToListAsync();
        }

        public async Task<decimal> GetCityAverageSalePriceAsync(int cityId)
        {
            var cityPrice = await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => x.Scope == MarketStatScope.City && x.CityId == cityId)
                .Select(x => (decimal?)x.BaseSalePricePerSqm)
                .FirstOrDefaultAsync();

            if (cityPrice.HasValue)
                return cityPrice.Value;

            var districtAverage = await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => x.Scope == MarketStatScope.District && x.CityId == cityId)
                .Select(x => (decimal?)x.BaseSalePricePerSqm)
                .AverageAsync();

            if (districtAverage.HasValue)
                return districtAverage.Value;

            var neighborhoodAverage = await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => x.Scope == MarketStatScope.Neighborhood && x.CityId == cityId)
                .Select(x => (decimal?)x.BaseSalePricePerSqm)
                .AverageAsync();

            return neighborhoodAverage ?? 0m;
        }
    }
}
