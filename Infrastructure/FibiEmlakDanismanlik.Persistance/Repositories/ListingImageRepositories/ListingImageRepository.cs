using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
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

            var sql = $"SELECT COUNT(1) FROM [{mapping.Table}] WHERE [{mapping.KeyColumn}] = @id";
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
        public async Task<ListingImage?> GetByIdAsync(int listingImageId)
        {
            return await _context.ListingImages
                .FirstOrDefaultAsync(x => x.ListingImageId == listingImageId);
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
                img.IsCover = (img.ListingImageId == coverImageId);
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

            // Kapak: IsCover olanlardan sıralamada ilk geleni koru, hiç yoksa ilk görsel kapak
            var firstCover = images.FirstOrDefault(x => x.IsCover);
            if (firstCover == null)
            {
                firstCover = images[0];
            }

            foreach (var img in images)
            {
                img.IsCover = ReferenceEquals(img, firstCover);
            }

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SyncLegacyColumnsAsync(int listingTypeId, int listingId)
        {
            if (!ListingImageConstants.ListingTables.TryGetValue(listingTypeId, out var mapping))
            {
                throw new ArgumentOutOfRangeException(nameof(listingTypeId), listingTypeId, "Bilinmeyen ilan türü kimliği.");
            }

            // Kapak önce, ardından SortOrder sırasıyla oku
            var images = await _context.ListingImages
                .AsNoTracking()
                .Where(x => x.ListingTypeId == listingTypeId && x.ListingId == listingId)
                .OrderByDescending(x => x.IsCover)
                .ThenBy(x => x.SortOrder)
                .Take(ListingImageConstants.MaxImagesPerListing)
                .Select(x => x.Url)
                .ToListAsync();

            var parameters = new List<SqlParameter>(ListingImageConstants.MaxImagesPerListing + 1);
            var setClauses = new List<string>(ListingImageConstants.MaxImagesPerListing);

            for (int i = 1; i <= ListingImageConstants.MaxImagesPerListing; i++)
            {
                var paramName = $"@p{i}";
                setClauses.Add($"[PropImgUrl{i}] = {paramName}");

                string? url = (i - 1 < images.Count) ? images[i - 1] : null;

                // PropImgUrl1 veritabanında NOT NULL
                object paramValue;
                if (url != null)
                {
                    paramValue = url;
                }
                else if (i == 1)
                {
                    paramValue = string.Empty;
                }
                else
                {
                    paramValue = DBNull.Value;
                }

                var param = new SqlParameter(paramName, System.Data.SqlDbType.NVarChar, 1024)
                {
                    Value = paramValue
                };
                parameters.Add(param);
            }

            parameters.Add(new SqlParameter("@id", listingId));

            var sql = $"UPDATE [{mapping.Table}] SET {string.Join(", ", setClauses)} WHERE [{mapping.KeyColumn}] = @id";

            await _context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }
    }
}
