using System.Text;
using System.Xml.Linq;
using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class SeoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SeoController> _logger;

        private const string SitemapCacheKey = "SitemapXml";

        public SeoController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ILogger<SeoController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpGet("robots.txt")]
        public IActionResult Robots()
        {
            var sitemapUrl = SeoUrl.Absolute(HttpContext, "/sitemap.xml");
            var content =
                "User-agent: *\n" +
                "Disallow: /Compare\n" +
                "Disallow: /Error/\n" +
                "Disallow: /blog/suggest\n" +
                "Disallow: /ThemeScaff\n" +
                "Allow: /\n\n" +
                $"Sitemap: {sitemapUrl}\n";

            return Content(content, "text/plain; charset=utf-8");
        }

        [HttpGet("sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            if (_memoryCache.TryGetValue(SitemapCacheKey, out string? cachedXml) && !string.IsNullOrEmpty(cachedXml))
            {
                return Content(cachedXml, "application/xml; charset=utf-8");
            }

            var hasApiError = false;
            var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var entries = new List<SitemapEntry>();

            void AddEntry(string? path, string? lastmod = null)
            {
                if (string.IsNullOrWhiteSpace(path))
                    return;

                var absoluteUrl = SeoUrl.Absolute(HttpContext, path);
                if (seenUrls.Add(absoluteUrl))
                {
                    entries.Add(new SitemapEntry
                    {
                        Loc = absoluteUrl,
                        LastMod = lastmod
                    });
                }
            }

            // 1) Sabit sayfalar
            AddEntry("/");
            AddEntry(ListingUrl.ListPath(false));
            AddEntry(ListingUrl.ListPath(true));
            AddEntry("/blog");
            AddEntry(Url.Action("Index", "AboutUs"));
            AddEntry(Url.Action("Index", "Contact"));
            AddEntry(Url.Action("Index", "FrequentlyAskedQuestion"));
            AddEntry(Url.Action("Index", "Agent"));
            AddEntry(Url.Action("Index", "ListingsMap"));
            AddEntry(Url.Action("RiskAnalysis", "FibiServices"));
            AddEntry(Url.Action("NeighborhoodAnalysis", "FibiServices"));
            AddEntry(Url.Action("Terms", "Legal"));
            AddEntry(Url.Action("Privacy", "Legal"));
            AddEntry(Url.Action("Kvkk", "Legal"));

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"] ?? string.Empty;
            if (!apiUrl.EndsWith('/'))
            {
                apiUrl += "/";
            }

            // 2) İlanlar ve il/ilçe açılış sayfaları
            try
            {
                var mapResponse = await client.GetAsync($"{apiUrl}Map/listings");
                if (mapResponse.IsSuccessStatusCode)
                {
                    var mapJson = await mapResponse.Content.ReadAsStringAsync();
                    var listings = JsonConvert.DeserializeObject<List<MapListingItemDto>>(mapJson);
                    if (listings != null)
                    {
                        foreach (var listing in listings)
                        {
                            var isRent = listing.UsageTypeId == 2;
                            var detailPath = ListingUrl.DetailPath(
                                isRent,
                                listing.ListingId,
                                listing.ListingTypeId,
                                listing.ListingTypeName,
                                listing.Title,
                                listing.District);
                            AddEntry(detailPath);

                            if (!string.IsNullOrWhiteSpace(listing.City))
                            {
                                AddEntry(ListingUrl.ListPath(isRent, listing.City.Trim()));

                                if (!string.IsNullOrWhiteSpace(listing.District))
                                {
                                    AddEntry(ListingUrl.ListPath(isRent, listing.City.Trim(), listing.District.Trim()));
                                }
                            }

                            var category = ListingUrl.CategorySlug(listing.ListingTypeId, listing.ListingTypeName);
                            if (category is "konut" or "arsa" or "isyeri")
                            {
                                AddEntry(ListingUrl.ListPath(isRent, null, null, category));

                                if (!string.IsNullOrWhiteSpace(listing.City))
                                {
                                    AddEntry(ListingUrl.ListPath(isRent, listing.City.Trim(), null, category));

                                    if (!string.IsNullOrWhiteSpace(listing.District))
                                    {
                                        AddEntry(ListingUrl.ListPath(isRent, listing.City.Trim(), listing.District.Trim(), category));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    hasApiError = true;
                    _logger.LogWarning("Sitemap: Harita ilanları API'den alınamadı. Durum kodu: {StatusCode}", mapResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                hasApiError = true;
                _logger.LogWarning(ex, "Sitemap: Harita ilanları API çağrısı sırasında hata oluştu.");
            }

            // 3) Bloglar
            try
            {
                var blogResponse = await client.GetAsync($"{apiUrl}Blog");
                if (blogResponse.IsSuccessStatusCode)
                {
                    var blogJson = await blogResponse.Content.ReadAsStringAsync();
                    var blogs = JsonConvert.DeserializeObject<List<ResultAllBlogsDto>>(blogJson);
                    if (blogs != null)
                    {
                        foreach (var blog in blogs)
                        {
                            var detailPath = BlogUrl.DetailPath(blog.BlogId, blog.BlogTitle);
                            string? lastmod = blog.CreatedDate != DateTime.MinValue
                                ? blog.CreatedDate.ToString("yyyy-MM-dd")
                                : null;
                            AddEntry(detailPath, lastmod);
                        }
                    }
                }
                else
                {
                    hasApiError = true;
                    _logger.LogWarning("Sitemap: Bloglar API'den alınamadı. Durum kodu: {StatusCode}", blogResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                hasApiError = true;
                _logger.LogWarning(ex, "Sitemap: Bloglar API çağrısı sırasında hata oluştu.");
            }

            // 4) XML Belgesi Oluşturma (sitemaps.org 0.9)
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urlElements = new List<XElement>();
            foreach (var entry in entries)
            {
                var elements = new List<object>
                {
                    new XElement(ns + "loc", entry.Loc)
                };

                if (!string.IsNullOrWhiteSpace(entry.LastMod))
                {
                    elements.Add(new XElement(ns + "lastmod", entry.LastMod));
                }

                urlElements.Add(new XElement(ns + "url", elements));
            }

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(ns + "urlset", urlElements)
            );

            string xmlContent;
            using (var writer = new Utf8StringWriter())
            {
                doc.Save(writer);
                xmlContent = writer.ToString();
            }

            var cacheDuration = hasApiError ? TimeSpan.FromMinutes(5) : TimeSpan.FromHours(1);
            _memoryCache.Set(SitemapCacheKey, xmlContent, cacheDuration);

            return Content(xmlContent, "application/xml; charset=utf-8");
        }

        private sealed class MapListingItemDto
        {
            public int ListingId { get; set; }
            public int ListingTypeId { get; set; }
            public int UsageTypeId { get; set; }
            public string? ListingTypeName { get; set; }
            public string? Title { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
        }

        private sealed class SitemapEntry
        {
            public string Loc { get; set; } = string.Empty;
            public string? LastMod { get; set; }
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
