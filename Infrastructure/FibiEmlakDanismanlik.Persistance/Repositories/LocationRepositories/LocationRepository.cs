using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FibiEmlakDanismanlik.Persistence.Repositories.LocationRepositories
{
    
    public class LocationRepository : ILocationRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public LocationRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<List<LocationOptionResult>> GetCitiesAsync(string? q)
        {
            var query = _context.Cities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Name.Contains(q));

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new LocationOptionResult(x.CityId, x.Name))
                .ToListAsync();
        }

        public async Task<List<LocationOptionResult>> GetDistrictsAsync(int cityId, string? q)
        {
            var query = _context.Districts.AsNoTracking()
                 .Where(x => x.CityId == cityId);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Name.Contains(q));

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new LocationOptionResult(x.DistrictId, x.Name))
                .ToListAsync();
        }

        public async Task<List<LocationOptionResult>> GetNeighborhoodsAsync(int districtId, string? q)
        {
            var query = _context.Neighborhoods.AsNoTracking()
               .Where(x => x.DistrictId == districtId);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Name.Contains(q));

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new LocationOptionResult(x.NeighborhoodId, x.Name))
                .ToListAsync();
        }
    }
}
