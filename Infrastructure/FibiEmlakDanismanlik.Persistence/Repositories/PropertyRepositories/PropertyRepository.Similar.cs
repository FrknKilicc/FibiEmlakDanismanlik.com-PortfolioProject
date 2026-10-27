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
using FibiEmlakDanismanlik.Persistence.Migrations;
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
        public async Task<SimilarPropertyReferenceResult?> GetForSaleSimilarReferenceAsync(int listingId)
        {
            return await BuildForSaleListingQuery()
         .AsNoTracking()
         .Where(x => x.ListingId == listingId)
         .Select(x => new SimilarPropertyReferenceResult
         {
             ListingId = x.ListingId,
             ListingTypeId = x.ListingTypeId,
             UsageTypeId = x.UsageTypeId,
             ListingType = x.ListingType,

             City = x.City,
             District = x.District,
             Neighborhood = x.Neighborhood,

             Price = x.Price,
             SquareMeter = x.ListingType == "Konut"
                 ? x.GrossArea
                 : x.Area,

             NumberOfRoom = x.NumberOfRoom
         })
         .FirstOrDefaultAsync();
        }

        public async Task<List<SimilarPropertyCandidateResult>> GetForSaleSimilarCandidatesAsync(SimilarPropertyReferenceResult reference, int take, int relaxLevel = 0)
        {
            if (reference == null)
                return new List<SimilarPropertyCandidateResult>();

            var minPrice = reference.Price.HasValue ? reference.Price.Value * 0.70m : (decimal?)null;
            var maxPrice = reference.Price.HasValue ? reference.Price.Value * 1.30m : (decimal?)null;

            var minSquareMeter = reference.SquareMeter.HasValue ? reference.SquareMeter.Value * 0.70 : (double?)null;
            var maxSquareMeter = reference.SquareMeter.HasValue ? reference.SquareMeter.Value * 1.30 : (double?)null;

            var query = BuildForSaleListingQuery()
                .AsNoTracking()
                .Where(x => x.ListingId != reference.ListingId)
                .Where(x => x.UsageTypeId == reference.UsageTypeId)
                .Where(x => x.ListingTypeId == reference.ListingTypeId);

            if (relaxLevel <= 2 && !string.IsNullOrWhiteSpace(reference.City))
            {
                query = query.Where(x => x.City == reference.City);
            }

            if (relaxLevel == 0 && minPrice.HasValue && maxPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value && x.Price <= maxPrice.Value);
            }

            if (relaxLevel <= 1 && minSquareMeter.HasValue && maxSquareMeter.HasValue)
            {
                query = query.Where(x =>
                    ((x.ListingType == "Konut" ? x.GrossArea : x.Area) ?? 0) >= minSquareMeter.Value &&
                    ((x.ListingType == "Konut" ? x.GrossArea : x.Area) ?? 0) <= maxSquareMeter.Value);
            }

            var candidates = await query
                .OrderByDescending(x => x.Neighborhood == reference.Neighborhood)
                .ThenByDescending(x => x.District == reference.District)
                .ThenByDescending(x => x.City == reference.City)
                .ThenByDescending(x => x.CreatedDate)
                .Take(take)
                .Select(x => new SimilarPropertyCandidateResult
                {
                    ListingId = x.ListingId,
                    ListingTypeId = x.ListingTypeId,
                    UsageTypeId = x.UsageTypeId,

                    ListingType = x.ListingType,
                    PropertyName = x.PropertyName,
                    PropertyDescription = x.PropertyDescription,

                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,

                    Price = x.Price,
                    SquareMeter = x.ListingType == "Konut" ? x.GrossArea : x.Area,
                    NumberOfRoom = x.NumberOfRoom,

                    CoverImageUrl = x.PropImgUrl1,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();

            return candidates;
        }

        private IQueryable<SimilarPropertyCandidateResult> BuildForRentalSimilarQuery()
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var housingQuery =
                from h in _context.rentalHousingListings
                join lt in _context.listingTypes on h.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && h.PropertyStatus != null
                      && activeStatuses.Contains(h.PropertyStatus)
                select new SimilarPropertyCandidateResult
                {
                    ListingId = h.RentalHousingListId,
                    ListingTypeId = h.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,
                    AgentName = h.Agent.AgentName,
                    AgentImageUrl = h.Agent.AgentImgUrl,

                    PropertyStatus = h.PropertyStatus,

                    ListingType = h.ListingType.Name,
                    PropertyName = h.PropertyName,
                    PropertyDescription = h.PropertyDescription,

                    City = h.City,
                    District = h.District,
                    Neighborhood = h.Neighborhood,

                    Price = h.Rent,
                    SquareMeter = h.GrossArea,
                    NumberOfRoom = h.NumberOfRoom,

                    CoverImageUrl = h.PropImgUrl1,
                    CreatedDate = h.CreatedDate
                };

            var landQuery =
                from l in _context.rentalLandListings
                join lt in _context.listingTypes on l.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && l.PropertyStatus != null
                      && activeStatuses.Contains(l.PropertyStatus)
                select new SimilarPropertyCandidateResult
                {
                    ListingId = l.RentalLandListingId,
                    ListingTypeId = l.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,

                    ListingType = l.ListingType.Name,
                    PropertyName = l.PropertyName,
                    PropertyDescription = l.PropertyDescription,

                    PropertyStatus=l.PropertyStatus,

                    AgentName = l.Agent.AgentName,
                    AgentImageUrl=l.Agent.AgentImgUrl,


                    City = l.City,
                    District = l.District,
                    Neighborhood = l.Neighborhood,

                    Price = l.Rent,
                    SquareMeter = l.Area,
                    NumberOfRoom = null,

                    CoverImageUrl = l.PropImgUrl1,
                    CreatedDate = l.CreatedDate
                };

            var commercialQuery =
                from c in _context.rentalCommercialPropertyListings
                join lt in _context.listingTypes on c.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && c.PropertyStatus != null
                      && activeStatuses.Contains(c.PropertyStatus)
                select new SimilarPropertyCandidateResult
                {
                    ListingId = c.RentalCommercialListId,
                    ListingTypeId = c.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,

                    ListingType = c.ListingType.Name,
                    PropertyName = c.PropertyName,
                    PropertyDescription = c.PropertyDescription,

                    AgentImageUrl = c.Agent.AgentImgUrl,
                    AgentName= c.Agent.AgentName,

                    PropertyStatus = c.PropertyStatus,

                    City = c.City,
                    District = c.District,
                    Neighborhood = c.Neighborhood,

                    Price = c.Rent,
                    SquareMeter = c.GrossArea,
                    NumberOfRoom = null,

                    CoverImageUrl = c.PropImgUrl1,
                    CreatedDate = c.CreatedDate
                };

            return housingQuery.Concat(landQuery).Concat(commercialQuery);
        }

        public async Task<SimilarPropertyReferenceResult?> GetForRentalSimilarReferenceAsync(int listingId)
        {
            return await BuildForRentalSimilarQuery()
                .AsNoTracking()
                .Where(x => x.ListingId == listingId)
                .Select(x => new SimilarPropertyReferenceResult
                {
                    ListingId = x.ListingId,
                    ListingTypeId = x.ListingTypeId,
                    UsageTypeId = x.UsageTypeId,
                    ListingType = x.ListingType,

                    

                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,

                    Price = x.Price,
                    SquareMeter = x.SquareMeter,
                    NumberOfRoom = x.NumberOfRoom
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<SimilarPropertyCandidateResult>> GetForRentalSimilarCandidatesAsync(
            SimilarPropertyReferenceResult reference,
            int take,
            int relaxLevel = 0)
        {
            if (reference == null)
                return new List<SimilarPropertyCandidateResult>();

            var minPrice = reference.Price.HasValue ? reference.Price.Value * 0.70m : (decimal?)null;
            var maxPrice = reference.Price.HasValue ? reference.Price.Value * 1.30m : (decimal?)null;

            var minSquareMeter = reference.SquareMeter.HasValue ? reference.SquareMeter.Value * 0.70 : (double?)null;
            var maxSquareMeter = reference.SquareMeter.HasValue ? reference.SquareMeter.Value * 1.30 : (double?)null;

            var query = BuildForRentalSimilarQuery()
                .AsNoTracking()
                .Where(x => x.ListingId != reference.ListingId)
                .Where(x => x.UsageTypeId == reference.UsageTypeId)
                .Where(x => x.ListingTypeId == reference.ListingTypeId);

            if (relaxLevel <= 2 && !string.IsNullOrWhiteSpace(reference.City))
            {
                query = query.Where(x => x.City == reference.City);
            }

            if (relaxLevel == 0 && minPrice.HasValue && maxPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value && x.Price <= maxPrice.Value);
            }

            if (relaxLevel <= 1 && minSquareMeter.HasValue && maxSquareMeter.HasValue)
            {
                query = query.Where(x =>
                    (x.SquareMeter ?? 0) >= minSquareMeter.Value &&
                    (x.SquareMeter ?? 0) <= maxSquareMeter.Value);
            }

            return await query
                .OrderByDescending(x => x.Neighborhood == reference.Neighborhood)
                .ThenByDescending(x => x.District == reference.District)
                .ThenByDescending(x => x.City == reference.City)
                .ThenByDescending(x => x.CreatedDate)
                .Take(take)
                .ToListAsync();
        }
    }
}
