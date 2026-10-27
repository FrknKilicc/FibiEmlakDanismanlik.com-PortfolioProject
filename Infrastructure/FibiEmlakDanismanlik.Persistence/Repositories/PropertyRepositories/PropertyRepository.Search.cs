using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.DTOs;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories
{
    public partial class PropertyRepository
    {
        public async Task<System.Collections.Generic.List<FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto>> GetPropertySuggestionsAsync(int usageType, int? listingTypeId, string searchTerm)
        {
            var results = new System.Collections.Generic.List<FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto>();
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Trim().Length < 3)
                return results;

            var term = searchTerm.Trim();
            var activeStatuses = PropertyStatuses.ActiveSet;

            if (usageType == (int)UsageType.ForSale)
            {
                var q1 = from x in _context.forSaleHousingPropertyListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForSale
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.ForSaleHousingListId, Title = x.PropertyNo + " - " + x.PropertyName };

                var q2 = from x in _context.forSaleLandListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForSale
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.ForSaleLandListingId, Title = x.PropertyNo + " - " + x.PropertyName };

                var q3 = from x in _context.forSaleCommercialPropertyListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForSale
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.ForSaleCommercialListingId, Title = x.PropertyNo + " - " + x.PropertyName };

                results.AddRange(await q1.Take(5).ToListAsync());
                results.AddRange(await q2.Take(5).ToListAsync());
                results.AddRange(await q3.Take(5).ToListAsync());
            }
            else if (usageType == (int)UsageType.ForRent)
            {
                var q1 = from x in _context.rentalHousingListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForRent
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.RentalHousingListId, Title = x.PropertyNo + " - " + x.PropertyName };

                var q2 = from x in _context.rentalLandListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForRent
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.RentalLandListingId, Title = x.PropertyNo + " - " + x.PropertyName };

                var q3 = from x in _context.rentalCommercialPropertyListings.AsNoTracking()
                         join lt in _context.listingTypes.AsNoTracking() on x.ListingTypeId equals lt.ListingTypeId
                         where lt.UsageType == UsageType.ForRent
                               && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                               && (!listingTypeId.HasValue || x.ListingTypeId == listingTypeId.Value)
                               && (x.PropertyName.Contains(term) || (x.PropertyNo != null && x.PropertyNo.ToString() == term))
                         select new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto { Id = x.RentalCommercialListId, Title = x.PropertyNo + " - " + x.PropertyName };

                results.AddRange(await q1.Take(5).ToListAsync());
                results.AddRange(await q2.Take(5).ToListAsync());
                results.AddRange(await q3.Take(5).ToListAsync());
            }

            return results.Take(10).ToList();
        }

        private sealed class FacetProjection
        {
            public decimal? Price { get; set; }
            public double? Area { get; set; }
            public string? Room { get; set; }
            public int? Bathroom { get; set; }
            public int? Floor { get; set; }
            public int? Section { get; set; }
            public string? ZoningStatus { get; set; }
            public int? AgentId { get; set; }
            public string? AgentName { get; set; }
            public int? HousingCategoryId { get; set; }
            public int? LandCategoryId { get; set; }
            public string? BuildingAge { get; set; }
            public string? Heating { get; set; }
            public string? TitleDeedStatus { get; set; }
        }

        public async Task<FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.SearchFilterFacetsResult> GetSearchFilterFacetsAsync(
            int usageType,
            int listingTypeId,
            int? cityId,
            int? districtId)
        {
            var result = new FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.SearchFilterFacetsResult();

            if (!Enum.IsDefined(typeof(UsageType), usageType))
                return result;

            string? cityName = null;
            if (cityId.HasValue)
            {
                cityName = await _context.Cities
                    .AsNoTracking()
                    .Where(c => c.CityId == cityId.Value)
                    .Select(c => c.Name.Trim())
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(cityName))
                    return result;
            }

            string? districtName = null;
            if (districtId.HasValue)
            {
                districtName = await _context.Districts
                    .AsNoTracking()
                    .Where(d => d.DistrictId == districtId.Value && (!cityId.HasValue || d.CityId == cityId.Value))
                    .Select(d => d.Name.Trim())
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(districtName))
                    return result;
            }

            var activeStatuses = PropertyStatuses.ActiveSet;
            var typeEnum = (PropertyListingType)listingTypeId;
            IQueryable<FacetProjection>? query = null;

            if (usageType == (int)UsageType.ForSale)
            {
                switch (typeEnum)
                {
                    case PropertyListingType.ForSaleHousingListing:
                    {
                        var q = BuildForSaleListingQuery()
                            .AsNoTracking()
                            .Where(x => x.ListingTypeId == listingTypeId);
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.GrossArea,
                            Room = x.NumberOfRoom,
                            Bathroom = x.NumberOfBathRoom,
                            Floor = x.NumberOfFloors,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            HousingCategoryId = x.HousingCategoryId,
                            BuildingAge = x.BuildingAge,
                            Heating = x.Heating
                        });
                        break;
                    }
                    case PropertyListingType.ForSaleLandListing:
                    {
                        var q = BuildForSaleListingQuery()
                            .AsNoTracking()
                            .Where(x => x.ListingTypeId == listingTypeId);
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.Area,
                            ZoningStatus = x.ZoningStatus,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            LandCategoryId = x.LandCategoryId,
                            TitleDeedStatus = x.TitleDeedStatus
                        });
                        break;
                    }
                    case PropertyListingType.ForSaleCommercialPropertyListing:
                    {
                        var q =
                            from comm in _context.forSaleCommercialPropertyListings.AsNoTracking()
                            join agent in _context.Agents.AsNoTracking() on comm.AgentId equals agent.AgentId
                            join lt in _context.listingTypes.AsNoTracking() on comm.ListingTypeId equals lt.ListingTypeId
                            where lt.UsageType == UsageType.ForSale
                                  && comm.ListingTypeId == listingTypeId
                                  && comm.PropertyStatus != null
                                  && activeStatuses.Contains(comm.PropertyStatus)
                            select new
                            {
                                comm.City,
                                comm.District,
                                comm.Price,
                                comm.GrossArea,
                                comm.NumberOfSection,
                                comm.NumberOfBathrooms,
                                comm.NumberOfFloors,
                                comm.TitleDeedStatus,
                                comm.AgentId,
                                agent.AgentName
                            };
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.GrossArea,
                            Section = x.NumberOfSection,
                            Bathroom = x.NumberOfBathrooms,
                            Floor = x.NumberOfFloors,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            TitleDeedStatus = x.TitleDeedStatus
                        });
                        break;
                    }
                }
            }
            else if (usageType == (int)UsageType.ForRent)
            {
                var baseFilter = new PropertyFilterRequest { ListingTypeIds = new List<int> { listingTypeId } };
                switch (typeEnum)
                {
                    case PropertyListingType.RentalHousingListing:
                    {
                        var q = BuildRentalHousingQuery(baseFilter).AsNoTracking();
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.GrossArea,
                            Room = x.NumberOfRoom,
                            Bathroom = x.NumberOfBathRoom,
                            Floor = x.NumberOfFloors,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            HousingCategoryId = x.HousingCategoryId,
                            BuildingAge = x.BuildingAge,
                            Heating = x.Heating
                        });
                        break;
                    }
                    case PropertyListingType.RentalLandListing:
                    {
                        var q = BuildRentalLandQuery(baseFilter).AsNoTracking();
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.Area,
                            ZoningStatus = x.ZoningStatus,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            LandCategoryId = x.LandCategoryId,
                            TitleDeedStatus = x.TitleDeedStatus
                        });
                        break;
                    }
                    case PropertyListingType.RentalCommercialPropertyListing:
                    {
                        var q = BuildRentalCommercialQuery(baseFilter).AsNoTracking();
                        if (cityName != null)
                            q = q.Where(x => x.City != null && x.City.Trim() == cityName);
                        if (districtName != null)
                            q = q.Where(x => x.District != null && x.District.Trim() == districtName);
                        query = q.Select(x => new FacetProjection
                        {
                            Price = x.Price,
                            Area = x.GrossArea,
                            Section = x.NumberOfSection,
                            Bathroom = x.NumberOfBathrooms,
                            Floor = x.NumberOfFloors,
                            AgentId = x.AgentId,
                            AgentName = x.AgentName,
                            BuildingAge = x.BuildingAge,
                            Heating = x.Heating
                        });
                        break;
                    }
                }
            }

            if (query == null)
                return result;

            var rows = await query.ToListAsync();
            result.TotalCount = rows.Count;
            if (rows.Count == 0)
                return result;

            var prices = rows.Where(r => r.Price.HasValue).Select(r => r.Price!.Value).ToList();
            if (prices.Count > 0)
            {
                result.MinPrice = prices.Min();
                result.MaxPrice = prices.Max();
            }

            var areas = rows.Where(r => r.Area.HasValue && r.Area.Value > 0).Select(r => r.Area!.Value).ToList();
            if (areas.Count > 0)
            {
                result.MinArea = areas.Min();
                result.MaxArea = areas.Max();
            }

            result.Rooms = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Room))
                .Select(r => r.Room!.Trim())
                .Distinct()
                .ToList();

            result.Bathrooms = rows
                .Where(r => r.Bathroom.HasValue)
                .Select(r => r.Bathroom!.Value)
                .Distinct()
                .ToList();

            result.Floors = rows
                .Where(r => r.Floor.HasValue)
                .Select(r => r.Floor!.Value)
                .Distinct()
                .ToList();

            result.Sections = rows
                .Where(r => r.Section.HasValue)
                .Select(r => r.Section!.Value)
                .Distinct()
                .ToList();

            result.BuildingAges = rows.Where(r => !string.IsNullOrWhiteSpace(r.BuildingAge)).Select(r => r.BuildingAge!.Trim()).Distinct().ToList();
            result.Heatings = rows.Where(r => !string.IsNullOrWhiteSpace(r.Heating)).Select(r => r.Heating!.Trim()).Distinct().ToList();
            result.TitleDeedStatuses = rows.Where(r => !string.IsNullOrWhiteSpace(r.TitleDeedStatus)).Select(r => r.TitleDeedStatus!.Trim()).Distinct().ToList();

            var housingCatIds = rows.Where(r => r.HousingCategoryId.HasValue).Select(r => r.HousingCategoryId!.Value).Distinct().ToList();
            if (housingCatIds.Count > 0)
                result.HousingCategories = await _context.HousingCategories.AsNoTracking()
                    .Where(c => housingCatIds.Contains(c.HousingCategoryId))
                    .Select(c => new FilterOptionDto(c.HousingCategoryId.ToString(), c.HousingCategoryName))
                    .ToListAsync();

            var landCatIds = rows.Where(r => r.LandCategoryId.HasValue).Select(r => r.LandCategoryId!.Value).Distinct().ToList();
            if (landCatIds.Count > 0)
                result.LandCategories = await _context.LandCategories.AsNoTracking()
                    .Where(c => landCatIds.Contains(c.LandCategoryId))
                    .Select(c => new FilterOptionDto(c.LandCategoryId.ToString(), c.LandCategoryName))
                    .ToListAsync();

            result.ZoningStatuses = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.ZoningStatus))
                .Select(r => r.ZoningStatus!.Trim())
                .Distinct()
                .ToList();

            result.Agents = rows
                .Where(r => r.AgentId.HasValue && !string.IsNullOrWhiteSpace(r.AgentName))
                .GroupBy(r => r.AgentId!.Value)
                .Select(g => new FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults.FilterOptionDto(
                    g.Key.ToString(),
                    g.First().AgentName!.Trim()))
                .ToList();

            return result;
        }
    }
}
