using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class ListingStructuredData
    {
        /// <summary>
        /// İlan detay sayfası için schema.org RealEstateListing biçiminde JSON-LD çıktısı üretir.
        /// </summary>
        public static string RealEstateListingJsonLd(
            HttpContext context,
            string canonicalPath,
            string? name,
            string? description,
            IEnumerable<string?> imageUrls,
            decimal price,
            bool isRent,
            DateTime? datePosted,
            string? city,
            string? district,
            string? neighborhood)
        {
            var root = new Dictionary<string, object>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "RealEstateListing"
            };

            if (!string.IsNullOrWhiteSpace(name))
            {
                root["name"] = name.Trim();
            }

            var desc = PageMeta.StripHtmlAndTruncate(description, 300);
            if (!string.IsNullOrWhiteSpace(desc))
            {
                root["description"] = desc;
            }

            if (!string.IsNullOrWhiteSpace(canonicalPath))
            {
                root["url"] = SeoUrl.Absolute(context, canonicalPath);
            }

            var validImages = (imageUrls ?? Enumerable.Empty<string?>())
                .Where(img => !string.IsNullOrWhiteSpace(img) && !string.Equals(img.Trim(), "string", StringComparison.OrdinalIgnoreCase))
                .Select(img =>
                {
                    var trimmed = img!.Trim();
                    if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        return trimmed;
                    }
                    return SeoUrl.Absolute(context, trimmed);
                })
                .Take(10)
                .ToList();

            if (validImages.Count > 0)
            {
                root["image"] = validImages;
            }

            if (datePosted.HasValue && datePosted.Value != DateTime.MinValue)
            {
                root["datePosted"] = datePosted.Value.ToString("yyyy-MM-dd");
            }

            var offers = new Dictionary<string, object>
            {
                ["@type"] = "Offer",
                ["price"] = price,
                ["priceCurrency"] = "TRY",
                ["availability"] = "https://schema.org/InStock"
            };

            if (isRent)
            {
                offers["priceSpecification"] = new Dictionary<string, object>
                {
                    ["@type"] = "UnitPriceSpecification",
                    ["price"] = price,
                    ["priceCurrency"] = "TRY",
                    ["unitCode"] = "MON"
                };
            }

            root["offers"] = offers;

            var address = new Dictionary<string, object>
            {
                ["@type"] = "PostalAddress"
            };

            if (!string.IsNullOrWhiteSpace(district))
            {
                address["addressLocality"] = district.Trim();
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                address["addressRegion"] = city.Trim();
            }

            if (!string.IsNullOrWhiteSpace(neighborhood))
            {
                address["streetAddress"] = $"{neighborhood.Trim()} Mah.";
            }

            address["addressCountry"] = "TR";

            root["contentLocation"] = new Dictionary<string, object>
            {
                ["@type"] = "Place",
                ["address"] = address
            };

            return JsonSerializer.Serialize(root);
        }
    }
}
