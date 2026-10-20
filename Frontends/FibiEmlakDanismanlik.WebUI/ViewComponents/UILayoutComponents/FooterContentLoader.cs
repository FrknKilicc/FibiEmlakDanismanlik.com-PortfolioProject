using FibiEmlakDanismanlik.Dto.AboutUsDtos;
using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.Dto.ContactDtos;
using FibiEmlakDanismanlik.Dto.SocialMediaDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Models.Footer;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents
{
    public static class FooterContentLoader
    {
        private const string CacheKey = "footer-layout";
        private const int CacheMinutes = 5;
        private const int MaxSummaryLength = 180;
        private const int MaxLatestBlogs = 2;

        public static async Task<FooterViewModel> LoadAsync(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            if (memoryCache.TryGetValue(CacheKey, out FooterViewModel? cached) && cached != null)
            {
                return cached;
            }

            var model = new FooterViewModel();
            var apiUrl = configuration["Url:ApiUrl"];
            var client = httpClientFactory.CreateClient();
            bool anySuccess = false;

            // Dört isteği paralel at
            var aboutTask = FetchAsync<ResultAboutUsDto>(client, $"{apiUrl}AboutUs/1");
            var contactTask = FetchAsync<List<ResultContactDto>>(client, $"{apiUrl}Contact");
            var socialTask = FetchAsync<List<ResultSocialMediaDto>>(client, $"{apiUrl}SocialMedia");
            var blogTask = FetchAsync<List<ResultBlogWithAuthorDto>>(client, $"{apiUrl}Blog/GetLast3BlogWithAuthor");

            await Task.WhenAll(aboutTask, contactTask, socialTask, blogTask);

            // Hakkımızda
            try
            {
                var about = await aboutTask;
                if (about != null && !string.IsNullOrWhiteSpace(about.AboutUsDesc))
                {
                    model.AboutSummary = PageMeta.StripHtmlAndTruncate(about.AboutUsDesc, MaxSummaryLength);
                    anySuccess = true;
                }
            }
            catch
            {
                // Hakkımızda verisi işlenemezse boş kalır
            }

            // İletişim
            try
            {
                var contacts = await contactTask;
                if (contacts != null && contacts.Count > 0)
                {
                    var contact = contacts[0];
                    model.Contact = contact;
                    var (display, href) = PhoneFormatter.Format(contact.PhoneNumber1);
                    model.PhoneDisplay = display;
                    model.PhoneHref = href;
                    anySuccess = true;
                }
            }
            catch
            {
                // İletişim verisi işlenemezse boş kalır
            }

            // Sosyal medya
            try
            {
                var socials = await socialTask;
                if (socials != null)
                {
                    model.SocialMedias = socials
                        .Where(s => !string.IsNullOrWhiteSpace(s.Url) &&
                                    (s.Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                     s.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                        .OrderBy(s => s.Order)
                        .ToList();
                    anySuccess = true;
                }
            }
            catch
            {
                // Sosyal medya verisi işlenemezse boş kalır
            }

            // Son haberler
            try
            {
                var blogs = await blogTask;
                if (blogs != null)
                {
                    model.LatestBlogs = blogs
                        .OrderByDescending(b => b.CreatedDate)
                        .Take(MaxLatestBlogs)
                        .ToList();
                    anySuccess = true;
                }
            }
            catch
            {
                // Blog verisi işlenemezse boş kalır
            }

            // Hiçbir istek başarılı olmadıysa önbelleğe alma
            if (anySuccess)
            {
                memoryCache.Set(CacheKey, model, TimeSpan.FromMinutes(CacheMinutes));
            }

            return model;
        }

        /// <summary>
        /// API'den veri çeker, başarısız olursa null döner.
        /// </summary>
        private static async Task<T?> FetchAsync<T>(HttpClient client, string url) where T : class
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch
            {
                // İstek başarısız olursa null dön
            }
            return null;
        }
    }
}
