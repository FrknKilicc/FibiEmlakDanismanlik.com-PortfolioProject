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
        private sealed class RawImageRow
        {
            public int ListingTypeId { get; set; }
            public int ListingId { get; set; }
            public string Url { get; set; } = null!;
            public bool IsCover { get; set; }
            public int SortOrder { get; set; }
            public int ListingImageId { get; set; }
        }

        private static bool TryPrepare(
            FibiEmlakDanismanlikContext context,
            IEnumerable<(int ListingTypeId, int ListingId)>? listings,
            out List<(int ListingTypeId, int ListingId)> pairs,
            out List<int> typeIds,
            out List<int> ids)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (listings == null)
            {
                pairs = null!;
                typeIds = null!;
                ids = null!;
                return false;
            }

            pairs = listings.Distinct().ToList();
            if (pairs.Count == 0)
            {
                typeIds = null!;
                ids = null!;
                return false;
            }

            typeIds = pairs.Select(x => x.ListingTypeId).Distinct().ToList();
            ids = pairs.Select(x => x.ListingId).Distinct().ToList();
            return true;
        }

        private static IQueryable<RawImageRow> BuildQuery(
            FibiEmlakDanismanlikContext context, List<int> typeIds, List<int> ids)
        {
            return context.ListingImages
                .AsNoTracking()
                .Where(x => typeIds.Contains(x.ListingTypeId) && ids.Contains(x.ListingId) && x.Kind == ListingImageKind.Photo)
                .Select(x => new RawImageRow
                {
                    ListingTypeId = x.ListingTypeId,
                    ListingId = x.ListingId,
                    Url = x.Url,
                    IsCover = x.IsCover,
                    SortOrder = x.SortOrder,
                    ListingImageId = x.ListingImageId
                });
        }

        private static Dictionary<(int ListingTypeId, int ListingId), List<string>> GroupImages(
            IEnumerable<RawImageRow> rawImages, HashSet<(int ListingTypeId, int ListingId)> requestedSet)
        {
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

        public static async Task<Dictionary<(int ListingTypeId, int ListingId), List<string>>> LoadAsync(
            FibiEmlakDanismanlikContext context, IEnumerable<(int ListingTypeId, int ListingId)> listings)
        {
            if (!TryPrepare(context, listings, out var pairs, out var typeIds, out var ids))
            {
                return new Dictionary<(int ListingTypeId, int ListingId), List<string>>();
            }

            var rawImages = await BuildQuery(context, typeIds, ids).ToListAsync();
            return GroupImages(rawImages, new HashSet<(int ListingTypeId, int ListingId)>(pairs));
        }

        public static Dictionary<(int ListingTypeId, int ListingId), List<string>> Load(
            FibiEmlakDanismanlikContext context, IEnumerable<(int ListingTypeId, int ListingId)> listings)
        {
            if (!TryPrepare(context, listings, out var pairs, out var typeIds, out var ids))
            {
                return new Dictionary<(int ListingTypeId, int ListingId), List<string>>();
            }

            var rawImages = BuildQuery(context, typeIds, ids).ToList();
            return GroupImages(rawImages, new HashSet<(int ListingTypeId, int ListingId)>(pairs));
        }
    }
}
