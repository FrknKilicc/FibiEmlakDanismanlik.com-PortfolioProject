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

        public async Task<List<LocationOptionResult>> GetAvailableCitiesAsync(int usageType, int listingTypeId, string? q)
        {
            var listingType = await _context.listingTypes.FindAsync(listingTypeId);
            if (listingType == null) return new List<LocationOptionResult>();

            List<string> validCityNames = new List<string>();

            if (usageType == 1) // Satılık
            {
                if (listingType.Name.Contains("Konut", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.forSaleHousingPropertyListings.Select(x => x.City).Distinct().ToListAsync();
                else if (listingType.Name.Contains("Arsa", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.forSaleLandListings.Select(x => x.City).Distinct().ToListAsync();
                else if (listingType.Name.Contains("İş", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.forSaleCommercialPropertyListings.Select(x => x.City).Distinct().ToListAsync();
            }
            else // Kiralık
            {
                if (listingType.Name.Contains("Konut", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.rentalHousingListings.Select(x => x.City).Distinct().ToListAsync();
                else if (listingType.Name.Contains("Arsa", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.rentalLandListings.Select(x => x.City).Distinct().ToListAsync();
                else if (listingType.Name.Contains("İş", StringComparison.OrdinalIgnoreCase))
                    validCityNames = await _context.rentalCommercialPropertyListings.Select(x => x.City).Distinct().ToListAsync();
            }

            var query = _context.Cities.AsNoTracking().Where(c => validCityNames.Contains(c.Name));

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Name.Contains(q));

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new LocationOptionResult(x.CityId, x.Name))
                .ToListAsync();
        }

        public async Task<List<LocationOptionResult>> GetAvailableDistrictsAsync(int cityId, int usageType, int listingTypeId, string? q)
        {
            var listingType = await _context.listingTypes.FindAsync(listingTypeId);
            var city = await _context.Cities.FindAsync(cityId);
            if (listingType == null || city == null) return new List<LocationOptionResult>();

            List<string> validDistrictNames = new List<string>();

            if (usageType == 1) // Satılık
            {
                if (listingType.Name.Contains("Konut", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.forSaleHousingPropertyListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
                else if (listingType.Name.Contains("Arsa", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.forSaleLandListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
                else if (listingType.Name.Contains("İş", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.forSaleCommercialPropertyListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
            }
            else // Kiralık
            {
                if (listingType.Name.Contains("Konut", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.rentalHousingListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
                else if (listingType.Name.Contains("Arsa", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.rentalLandListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
                else if (listingType.Name.Contains("İş", StringComparison.OrdinalIgnoreCase))
                    validDistrictNames = await _context.rentalCommercialPropertyListings.Where(x => x.City == city.Name).Select(x => x.District).Distinct().ToListAsync();
            }

            var query = _context.Districts.AsNoTracking().Where(d => d.CityId == cityId && validDistrictNames.Contains(d.Name));

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.Name.Contains(q));

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new LocationOptionResult(x.DistrictId, x.Name))
                .ToListAsync();
        }
    }
}
