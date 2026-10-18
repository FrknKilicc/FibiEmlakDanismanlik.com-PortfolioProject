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

        public async Task<ExternalMarketStat?> GetByNeighborhoodIdAsync(int neighborhoodId)
        {
            return await _context.ExternalMarketStats
                .AsNoTracking()
                .Include(x => x.Neighborhood!)
                    .ThenInclude(n => n.District)
                        .ThenInclude(d => d.City)
                .FirstOrDefaultAsync(x => x.Scope == MarketStatScope.Neighborhood && x.NeighborhoodId == neighborhoodId);
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
            var average = await _context.ExternalMarketStats
                .AsNoTracking()
                .Where(x => x.Scope == MarketStatScope.Neighborhood && x.CityId == cityId)
                .Select(x => (decimal?)x.BaseSalePricePerSqm)
                .AverageAsync();

            return average ?? 0m;
        }
    }
}
