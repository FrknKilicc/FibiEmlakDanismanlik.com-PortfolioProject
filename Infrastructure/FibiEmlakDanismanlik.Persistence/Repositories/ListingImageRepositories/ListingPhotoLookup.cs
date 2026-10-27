using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories
{
    /// <summary>İlanların fotoğraf adreslerini eski PropImgUrl sırasıyla (kapak, SortOrder, Id) tek sorguda getirir.</summary>
    public static class ListingPhotoLookup
    {
        public static async Task<Dictionary<(int ListingTypeId, int ListingId), List<string>>> LoadAsync(
            FibiEmlakDanismanlikContext context, IEnumerable<(int ListingTypeId, int ListingId)> listings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (listings == null)
            {
                return new Dictionary<(int ListingTypeId, int ListingId), List<string>>();
            }

            var pairs = listings.Distinct().ToList();
            if (pairs.Count == 0)
            {
                return new Dictionary<(int ListingTypeId, int ListingId), List<string>>();
            }

            var typeIds = pairs.Select(x => x.ListingTypeId).Distinct().ToList();
            var ids = pairs.Select(x => x.ListingId).Distinct().ToList();

            var rawImages = await context.ListingImages
                .AsNoTracking()
                .Where(x => typeIds.Contains(x.ListingTypeId) && ids.Contains(x.ListingId) && x.Kind == ListingImageKind.Photo)
                .Select(x => new
                {
                    x.ListingTypeId,
                    x.ListingId,
                    x.Url,
                    x.IsCover,
                    x.SortOrder,
                    x.ListingImageId
                })
                .ToListAsync();

            var requestedSet = new HashSet<(int ListingTypeId, int ListingId)>(pairs);

            return rawImages
                .Where(x => requestedSet.Contains((x.ListingTypeId, x.ListingId)))
                .GroupBy(x => (x.ListingTypeId, x.ListingId))
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.IsCover)
                          .ThenBy(x => x.SortOrder)
                          .ThenBy(x => x.ListingImageId)
                          .Take(ListingImageConstants.MaxImagesPerListing)
                          .Select(x => x.Url)
                          .ToList()
                );
        }
    }
}
