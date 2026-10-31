using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories
{
    /// <summary>
    /// İlan fotoğrafları için veri erişim ve eski kolonlarla çift yazma senkronizasyon repository sınıfı.
    /// </summary>
    public class ListingImageRepository : IListingImageRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ListingImageRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<bool> ListingExistsAsync(int listingTypeId, int listingId)
        {
            if (!ListingImageConstants.ListingTables.TryGetValue(listingTypeId, out var mapping))
            {
                throw new ArgumentOutOfRangeException(nameof(listingTypeId), listingTypeId, "Bilinmeyen ilan türü kimliği.");
            }

            var sql = $"SELECT COUNT(1) FROM [{mapping.Table}] WHERE [{mapping.KeyColumn}] = @id AND [DeletedAtUtc] IS NULL";
            var countList = await _context.Database
                .SqlQueryRaw<int>(sql, new SqlParameter("@id", listingId))
                .ToListAsync();

            return countList.Count > 0 && countList[0] > 0;
        }

        /// <inheritdoc />
        public async Task<List<ListingImage>> GetByListingAsync(int listingTypeId, int listingId)
        {
            return await _context.ListingImages
                .AsNoTracking()
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<ListingImage>> GetPlanImagesAsync(int listingTypeId, int listingId)
        {
            return await _context.ListingImages
                .AsNoTracking()
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId && x.Kind != ListingImageKind.Photo)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ListingImage?> GetByIdAsync(int listingImageId)
        {
            return await _context.ListingImages
                .FirstOrDefaultAsync(x => x.ListingImageId == listingImageId);
        }

        /// <inheritdoc />
        public async Task UpdateKindAsync(int listingImageId, ListingImageKind kind, string? caption)
        {
            var image = await _context.ListingImages.FirstOrDefaultAsync(x => x.ListingImageId == listingImageId);
            if (image == null)
            {
                return;
            }

            image.Kind = kind;
            image.Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddRangeAsync(IEnumerable<ListingImage> images)
        {
            await _context.ListingImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(ListingImage image)
        {
            _context.ListingImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveOrderAsync(int listingTypeId, int listingId, IReadOnlyList<int> orderedImageIds, int coverImageId)
        {
            var images = await _context.ListingImages
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .ToListAsync();

            if (images.Count == 0)
            {
                return;
            }

            var imagesMap = images.ToDictionary(x => x.ListingImageId);
            var orderedList = new List<ListingImage>(images.Count);

            if (orderedImageIds != null)
            {
                foreach (var id in orderedImageIds)
                {
                    if (imagesMap.Remove(id, out var img))
                    {
                        orderedList.Add(img);
                    }
                }
            }

            // Listede olmayan görseller mevcut sıralamalarıyla sona eklenir
            var remainingImages = imagesMap.Values.OrderBy(x => x.SortOrder).ToList();
            orderedList.AddRange(remainingImages);

            var currentSortOrder = 1;
            foreach (var img in orderedList)
            {
                img.SortOrder = currentSortOrder++;
                img.IsCover = img.ListingImageId == coverImageId && img.Kind == ListingImageKind.Photo;
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task NormalizeAsync(int listingTypeId, int listingId)
        {
            var images = await _context.ListingImages
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.ListingImageId)
                .ToListAsync();

            if (images.Count == 0)
            {
                return;
            }

            // SortOrder'ı 1'den kesintisiz yeniden ver
            int order = 1;
            foreach (var img in images)
            {
                img.SortOrder = order++;
            }

            // Kapak yalnızca Fotoğraf türünden seçilir
            var photos = images.Where(x => x.Kind == ListingImageKind.Photo).ToList();
            var firstCover = photos.FirstOrDefault(x => x.IsCover) ?? photos.FirstOrDefault();

            foreach (var img in images)
            {
                img.IsCover = ReferenceEquals(img, firstCover);
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Dictionary<(int ListingTypeId, int ListingId), List<string>>> GetPhotoUrlMapAsync(
            IEnumerable<(int ListingTypeId, int ListingId)> listings)
        {
            return await ListingPhotoLookup.LoadAsync(_context, listings);
        }

        /// <inheritdoc />
        public async Task ReplacePhotoUrlsAsync(int listingTypeId, int listingId, IReadOnlyList<string?> urls)
        {
            if (!ListingImageConstants.ListingTables.TryGetValue(listingTypeId, out _))
            {
                throw new ArgumentOutOfRangeException(nameof(listingTypeId), listingTypeId, "Bilinmeyen ilan türü kimliği.");
            }

            var cleanedUrls = new List<string>();
            if (urls != null)
            {
                var seen = new HashSet<string>();
                foreach (var rawUrl in urls)
                {
                    if (string.IsNullOrWhiteSpace(rawUrl))
                    {
                        continue;
                    }

                    var trimmed = rawUrl.Trim();
                    if (seen.Add(trimmed))
                    {
                        cleanedUrls.Add(trimmed);
                        if (cleanedUrls.Count == ListingImageConstants.MaxImagesPerListing)
                        {
                            break;
                        }
                    }
                }
            }

            var existingPhotos = await _context.ListingImages
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId && x.Kind == ListingImageKind.Photo)
                .ToListAsync();

            var targetUrlSet = new HashSet<string>(cleanedUrls);
            var existingByUrl = new Dictionary<string, ListingImage>();

            foreach (var photo in existingPhotos)
            {
                if (targetUrlSet.Contains(photo.Url) && !existingByUrl.ContainsKey(photo.Url))
                {
                    existingByUrl[photo.Url] = photo;
                }
                else
                {
                    _context.ListingImages.Remove(photo);
                }
            }

            for (int i = 0; i < cleanedUrls.Count; i++)
            {
                var url = cleanedUrls[i];
                var sortOrder = i + 1;
                var isCover = (i == 0);

                if (existingByUrl.TryGetValue(url, out var existingImage))
                {
                    existingImage.SortOrder = sortOrder;
                    existingImage.IsCover = isCover;
                }
                else
                {
                    var newImage = new ListingImage
                    {
                        ListingTypeId = listingTypeId,
                        ListingId = listingId,
                        Url = url,
                        Kind = ListingImageKind.Photo,
                        SortOrder = sortOrder,
                        IsCover = isCover,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    _context.ListingImages.Add(newImage);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
