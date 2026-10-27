using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.BestDealResults;
using FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.BestDealRepositories
{
    public class BestDealRepository : IBestDealRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public BestDealRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<List<GetBestDealResult>> GetBestDealsAsync(int usageType, int take)
        {
            if (take <= 0 || (usageType != 1 && usageType != 2))
            {
                return new List<GetBestDealResult>();
            }

            var activeStatuses = PropertyStatuses.ActiveSet;

            if (usageType == 1)
            {
                var housingDeals = await _context.forSaleHousingPropertyListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.ForSaleHousingListId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "Konut",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Price,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 2 && i.ListingId == x.ForSaleHousingListId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.GrossArea,
                        NumberOfRoom = x.NumberOfRoom,
                        NumberOfBathRoom = x.NumberOfBathRoom,
                        NumberOfSection = null,
                        ZoningStatus = null,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                var commercialDeals = await _context.forSaleCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.ForSaleCommercialListingId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "İşyeri",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Price,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 3 && i.ListingId == x.ForSaleCommercialListingId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.GrossArea,
                        NumberOfRoom = null,
                        NumberOfBathRoom = x.NumberOfBathrooms,
                        NumberOfSection = x.NumberOfSection,
                        ZoningStatus = null,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                var landDeals = await _context.forSaleLandListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.ForSaleLandListingId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "Arsa",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Price,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 1 && i.ListingId == x.ForSaleLandListingId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.Area,
                        NumberOfRoom = null,
                        NumberOfBathRoom = null,
                        NumberOfSection = null,
                        ZoningStatus = x.ZoningStatus,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                return housingDeals
                    .Concat(commercialDeals)
                    .Concat(landDeals)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .ToList();
            }
            else
            {
                var housingDeals = await _context.rentalHousingListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.RentalHousingListId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "Konut",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Rent,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 8 && i.ListingId == x.RentalHousingListId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.GrossArea,
                        NumberOfRoom = x.NumberOfRoom,
                        NumberOfBathRoom = x.NumberOfBathRoom,
                        NumberOfSection = null,
                        ZoningStatus = null,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                var commercialDeals = await _context.rentalCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.RentalCommercialListId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "İşyeri",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Rent,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 9 && i.ListingId == x.RentalCommercialListId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.GrossArea,
                        NumberOfRoom = null,
                        NumberOfBathRoom = x.NumberOfBathrooms,
                        NumberOfSection = x.NumberOfSection,
                        ZoningStatus = null,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                var landDeals = await _context.rentalLandListings
                    .AsNoTracking()
                    .Where(x => x.BestDeals == true && x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .Select(x => new GetBestDealResult
                    {
                        ListingId = x.RentalLandListingId,
                        UsageType = usageType,
                        ListingTypeId = x.ListingTypeId,
                        ListingTypeName = "Arsa",
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        Price = x.Rent,
                        City = x.City,
                        District = x.District,
                        Neighborhood = x.Neighborhood,
                        ImageUrl = _context.ListingImages
                            .Where(i => i.ListingTypeId == 7 && i.ListingId == x.RentalLandListingId && i.Kind == ListingImageKind.Photo)
                            .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                            .Select(i => i.Url)
                            .FirstOrDefault() ?? "",
                        Area = x.Area,
                        NumberOfRoom = null,
                        NumberOfBathRoom = null,
                        NumberOfSection = null,
                        ZoningStatus = x.ZoningStatus,
                        AgentName = x.Agent.AgentName,
                        AgentImgUrl = x.Agent.AgentImgUrl,
                        CreatedDate = x.CreatedDate
                    })
                    .ToListAsync();

                return housingDeals
                    .Concat(commercialDeals)
                    .Concat(landDeals)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .ToList();
            }
        }
    }
}
