using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Features.Results.PropertyLocationResults;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
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

        private sealed class ActiveLocationProjection
        {
            public string? City { get; set; }
            public string? District { get; set; }
        }

        public LocationRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        private IQueryable<ActiveLocationProjection>? BuildActiveLocationQuery(int usageType, int listingTypeId)
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            if (usageType == (int)UsageType.ForSale)
            {
                return (PropertyListingType)listingTypeId switch
                {
                    PropertyListingType.ForSaleLandListing =>
                        from x in _context.forSaleLandListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForSale
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    PropertyListingType.ForSaleHousingListing =>
                        from x in _context.forSaleHousingPropertyListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForSale
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    PropertyListingType.ForSaleCommercialPropertyListing =>
                        from x in _context.forSaleCommercialPropertyListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForSale
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    _ => null
                };
            }

            if (usageType == (int)UsageType.ForRent)
            {
                return (PropertyListingType)listingTypeId switch
                {
                    PropertyListingType.RentalLandListing =>
                        from x in _context.rentalLandListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForRent
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    PropertyListingType.RentalHousingListing =>
                        from x in _context.rentalHousingListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForRent
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    PropertyListingType.RentalCommercialPropertyListing =>
                        from x in _context.rentalCommercialPropertyListings.AsNoTracking()
                        join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                        where lt.UsageType == UsageType.ForRent
                              && x.ListingTypeId == listingTypeId
                              && x.PropertyStatus != null
                              && activeStatuses.Contains(x.PropertyStatus)
                        select new ActiveLocationProjection { City = x.City, District = x.District },

                    _ => null
                };
            }

            return null;
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
            var activeLocations = BuildActiveLocationQuery(usageType, listingTypeId);
            if (activeLocations == null)
                return new List<LocationOptionResult>();

            var query = _context.Cities.AsNoTracking()
                .Where(c => activeLocations.Any(p => p.City != null && p.City.Trim() == c.Name.Trim()));

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(c => c.Name.Contains(term));
            }

            return await query
                .OrderBy(c => c.Name)
                .Select(c => new LocationOptionResult(c.CityId, c.Name))
                .ToListAsync();
        }

        public async Task<List<LocationOptionResult>> GetAvailableDistrictsAsync(int cityId, int usageType, int listingTypeId, string? q)
        {
            var activeLocations = BuildActiveLocationQuery(usageType, listingTypeId);
            if (activeLocations == null)
                return new List<LocationOptionResult>();

            var query =
                from d in _context.Districts.AsNoTracking()
                join c in _context.Cities.AsNoTracking() on d.CityId equals c.CityId
                where d.CityId == cityId
                      && activeLocations.Any(p =>
                          p.City != null && p.City.Trim() == c.Name.Trim() &&
                          p.District != null && p.District.Trim() == d.Name.Trim())
                select d;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(d => d.Name.Contains(term));
            }

            return await query
                .OrderBy(d => d.Name)
                .Select(d => new LocationOptionResult(d.DistrictId, d.Name))
                .ToListAsync();
        }

        public async Task<List<string>> GetListingCitiesAsync(int usageType)
        {
            if (usageType == (int)UsageType.ForSale)
            {
                return await _context.forSaleHousingPropertyListings.AsNoTracking().Select(x => x.City)
                    .Union(_context.forSaleLandListings.AsNoTracking().Select(x => x.City))
                    .Union(_context.forSaleCommercialPropertyListings.AsNoTracking().Select(x => x.City))
                    .Distinct()
                    .ToListAsync();
            }

            if (usageType == (int)UsageType.ForRent)
            {
                return await _context.rentalHousingListings.AsNoTracking().Select(x => x.City)
                    .Union(_context.rentalLandListings.AsNoTracking().Select(x => x.City))
                    .Union(_context.rentalCommercialPropertyListings.AsNoTracking().Select(x => x.City))
                    .Distinct()
                    .ToListAsync();
            }

            return new List<string>();
        }

        public async Task<GetFilteredPropertyLocationListingsResponse> GetFilteredListingsByLocationAsync(
            int usageType,
            int listingTypeId,
            string? location,
            string? keyword)
        {
            var listingType = await _context.listingTypes.AsNoTracking().FirstOrDefaultAsync(x => x.ListingTypeId == listingTypeId);
            if (listingType == null)
            {
                return new GetFilteredPropertyLocationListingsResponse
                {
                    ListingTypeFound = false,
                    IsValidCombination = false,
                    Items = new List<GetFilteredPropertyLocationListingResult>()
                };
            }

            var propertyListingType = (PropertyListingType)listingTypeId;

            if (usageType == (int)UsageType.ForSale)
            {
                switch (propertyListingType)
                {
                    case PropertyListingType.ForSaleLandListing:
                    {
                        var query = _context.forSaleLandListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.ForSaleLandListingId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Price,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    case PropertyListingType.ForSaleHousingListing:
                    {
                        var query = _context.forSaleHousingPropertyListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.ForSaleHousingListId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Price,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    case PropertyListingType.ForSaleCommercialPropertyListing:
                    {
                        var query = _context.forSaleCommercialPropertyListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.ForSaleCommercialListingId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Price,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    default:
                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = false,
                            Items = new List<GetFilteredPropertyLocationListingResult>()
                        };
                }
            }

            if (usageType == (int)UsageType.ForRent)
            {
                switch (propertyListingType)
                {
                    case PropertyListingType.RentalLandListing:
                    {
                        var query = _context.rentalLandListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.RentalLandListingId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Rent,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    case PropertyListingType.RentalHousingListing:
                    {
                        var query = _context.rentalHousingListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.RentalHousingListId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Rent,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    case PropertyListingType.RentalCommercialPropertyListing:
                    {
                        var query = _context.rentalCommercialPropertyListings.AsNoTracking();
                        if (!string.IsNullOrEmpty(location))
                            query = query.Where(x => x.City.Contains(location));
                        if (!string.IsNullOrEmpty(keyword))
                            query = query.Where(x => x.PropertyName.Contains(keyword));

                        var items = await query.Select(x => new GetFilteredPropertyLocationListingResult
                        {
                            ListingId = x.RentalCommercialListId,
                            ListingTypeId = x.ListingTypeId,
                            PropertyName = x.PropertyName,
                            PropertyDescription = x.PropertyDescription,
                            City = x.City,
                            District = x.District,
                            Neighborhood = x.Neighborhood,
                            Price = x.Rent,
                            CoverImageUrl = x.PropImgUrl1,
                            PropertyStatus = x.PropertyStatus
                        }).ToListAsync();

                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = true,
                            Items = items
                        };
                    }

                    default:
                        return new GetFilteredPropertyLocationListingsResponse
                        {
                            ListingTypeFound = true,
                            IsValidCombination = false,
                            Items = new List<GetFilteredPropertyLocationListingResult>()
                        };
                }
            }

            return new GetFilteredPropertyLocationListingsResponse
            {
                ListingTypeFound = true,
                IsValidCombination = false,
                Items = new List<GetFilteredPropertyLocationListingResult>()
            };
        }
    }
}

