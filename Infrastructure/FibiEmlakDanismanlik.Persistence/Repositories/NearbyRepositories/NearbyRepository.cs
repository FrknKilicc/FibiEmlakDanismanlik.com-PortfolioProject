using FibiEmlakDanismanlik.Application.Features.Results.NearbyResults;
using FibiEmlakDanismanlik.Application.Interfaces.NearbyInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.NearbyRepositories
{
    public class NearbyRepository:INearbyRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public NearbyRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<List<NearbyCategoryGroupResult>> GetNearbyByListingAsync(int listingId, int ListingTypeId)
        {
            var rows = await _context.ListingNearbyPlaces
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.ListingId == listingId &&
                    x.ListingTypeId == ListingTypeId &&
                    x.NearbyCategory != null &&
                    x.NearbyCategory.IsActive)
                .Select(x => new
                {
                    CategoryName = x.NearbyCategory!.Name,
                    IconCss = x.NearbyCategory!.IconCss,
                    CategorySort = x.NearbyCategory!.SortOrder,
                    PlaceName = x.PlaceName,
                    DistanceKm = x.DistanceKm,
                    Stars = x.Stars,
                    ItemSort = x.SortOrder
                })
                .ToListAsync();

            var grouped = rows
                .GroupBy(r => new { r.CategoryName, r.IconCss, r.CategorySort })
                .OrderBy(g => g.Key.CategorySort)
                .Select(g => new NearbyCategoryGroupResult
                {
                    CategoryName = g.Key.CategoryName,
                    IconCss = g.Key.IconCss,
                    SortOrder = g.Key.CategorySort,
                    Items = g.OrderBy(x => x.DistanceKm)
                             .ThenBy(x => x.ItemSort)
                             .Take(3)
                             .Select(x => new NearbyPlaceItemResult
                             {
                                 PlaceName = x.PlaceName,
                                 DistanceKm = x.DistanceKm,
                                 Stars = x.Stars
                             })
                             .ToList()
                })
                .Where(x => x.Items.Count > 0)
                .ToList();

            return grouped;
        }
    }
}


