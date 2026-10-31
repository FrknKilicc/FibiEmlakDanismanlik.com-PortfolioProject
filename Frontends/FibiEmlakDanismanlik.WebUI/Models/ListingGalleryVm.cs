using System;
using System.Collections.Generic;
using System.Linq;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    /// <summary>
    /// İlan detay sayfası gelişmiş fotoğraf galerisi görünüm modeli.
    /// </summary>
    public class ListingGalleryVm
    {
        public List<string> ImageUrls { get; set; } = new();
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Ham görsel listesinden boş, "string" değerli ve tekrarlı olanları eleyerek normalize edilmiş galeri modeli üretir.
        /// </summary>
        public static ListingGalleryVm Create(IEnumerable<string?> rawUrls, string? title, Func<string, string> normalize)
        {
            if (rawUrls == null)
            {
                return new ListingGalleryVm
                {
                    ImageUrls = new List<string>(),
                    Title = title?.Trim() ?? string.Empty
                };
            }

            var cleanList = rawUrls
                .Where(x => !string.IsNullOrWhiteSpace(x) && !string.Equals(x.Trim(), "string", StringComparison.OrdinalIgnoreCase))
                .Select(x => normalize(x!.Trim()))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            return new ListingGalleryVm
            {
                ImageUrls = cleanList,
                Title = title?.Trim() ?? string.Empty
            };
        }
    }
}
