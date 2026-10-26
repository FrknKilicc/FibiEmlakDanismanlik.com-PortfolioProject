using FibiEmlakDanismanlik.Application.Constants;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Services
{
    /// <summary>
    /// Panelden düzenlenen site metinlerini (api/SiteTexts) kısa süreli önbellekle sunar; API'ye ulaşılamazsa varsayılanlar kullanılır.
    /// Görünümlerde: <c>@inject SiteTextService SiteTextService</c> + <c>var texts = await SiteTextService.GetAsync();</c>
    /// </summary>
    public class SiteTextService
    {
        private const string CacheKey = "site-texts";
        // Panel değişikliği en geç bu sürede görünür
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan FailureCacheDuration = TimeSpan.FromSeconds(10);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public SiteTextService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<SiteTexts> GetAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out SiteTexts? cached) && cached != null)
            {
                return cached;
            }

            Dictionary<string, string>? stored = null;
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}SiteTexts");
                if (response.IsSuccessStatusCode)
                {
                    stored = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                // API kapalıysa varsayılan metinlerle devam edilir
            }

            var texts = new SiteTexts(SiteTextDefinitions.Merge(stored ?? new Dictionary<string, string>()));
            _memoryCache.Set(CacheKey, texts, stored != null ? CacheDuration : FailureCacheDuration);
            return texts;
        }
    }

    /// <summary>
    /// Site metinleri sözlüğü; bilinmeyen anahtar boş döner.
    /// </summary>
    public sealed class SiteTexts
    {
        private readonly IReadOnlyDictionary<string, string> _values;

        public SiteTexts(IReadOnlyDictionary<string, string> values)
        {
            _values = values;
        }

        public string this[string key] => _values.TryGetValue(key, out var v) ? v : string.Empty;

        public int Int(string key)
        {
            if (int.TryParse(this[key], out var n)) return n;
            return int.TryParse(SiteTextDefinitions.Find(key)?.Default, out var d) ? d : 0;
        }
    }
}
