using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class SeoUrl
    {
        /// <summary>
        /// Verilen bağıl yolu, yapılandırmadaki SiteBaseUrl veya mevcut HttpContext tabanı ile mutlak URL'ye dönüştürür.
        /// </summary>
        public static string Absolute(HttpContext context, string path)
        {
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            var siteBaseUrl = configuration["Seo:SiteBaseUrl"]?.TrimEnd('/');

            string baseUrl;
            if (!string.IsNullOrWhiteSpace(siteBaseUrl))
            {
                baseUrl = siteBaseUrl;
            }
            else
            {
                baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            }

            var cleanPath = path ?? string.Empty;
            if (!cleanPath.StartsWith('/'))
            {
                cleanPath = "/" + cleanPath;
            }

            return baseUrl + cleanPath;
        }

        /// <summary>
        /// Verilen gezinme öğelerinden schema.org BreadcrumbList biçiminde JSON-LD çıktısı üretir.
        /// </summary>
        public static string BreadcrumbJsonLd(HttpContext context, params (string Name, string Path)[] items)
        {
            // Varsayılan JsonSerializerOptions kullanıldığında JavaScriptEncoder HTML karakterlerini (<, >, & vb.)
            // otomatik olarak kaçırır; bu sayede çıktı HTML script etiketi içine güvenle gömülebilir.
            var elements = items.Select((item, index) => new Dictionary<string, object>
            {
                ["@type"] = "ListItem",
                ["position"] = index + 1,
                ["name"] = item.Name,
                ["item"] = Absolute(context, item.Path)
            }).ToList();

            var root = new Dictionary<string, object>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "BreadcrumbList",
                ["itemListElement"] = elements
            };

            return JsonSerializer.Serialize(root);
        }
    }
}
