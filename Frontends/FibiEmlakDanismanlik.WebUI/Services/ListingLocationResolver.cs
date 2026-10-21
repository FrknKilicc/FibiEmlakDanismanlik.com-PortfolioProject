using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Services
{
    public class ListingLocationResolver
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public ListingLocationResolver(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<ListingLocationVm?> ResolveAsync(bool isRent, string citySlug, string? districtSlug)
        {
            if (string.IsNullOrWhiteSpace(citySlug))
                return null;

            const string citiesCacheKey = "listing-location:cities";
            if (!_memoryCache.TryGetValue(citiesCacheKey, out List<LocationOptionVm>? cities) || cities == null)
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["Url:ApiUrl"];
                var response = await client.GetAsync(apiUrl + "Location/cities");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                cities = JsonConvert.DeserializeObject<List<LocationOptionVm>>(json) ?? new List<LocationOptionVm>();
                _memoryCache.Set(citiesCacheKey, cities, TimeSpan.FromHours(6));
            }

            var city = cities.FirstOrDefault(x => ListingUrl.Slugify(x.Text) == citySlug.ToLowerInvariant());
            if (city == null)
                return null;

            LocationOptionVm? district = null;
            if (!string.IsNullOrWhiteSpace(districtSlug))
            {
                var districtsCacheKey = $"listing-location:districts:{city.Id}";
                if (!_memoryCache.TryGetValue(districtsCacheKey, out List<LocationOptionVm>? districts) || districts == null)
                {
                    var client = _httpClientFactory.CreateClient();
                    var apiUrl = _configuration["Url:ApiUrl"];
                    var response = await client.GetAsync(apiUrl + $"Location/districts?cityId={city.Id}");
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    districts = JsonConvert.DeserializeObject<List<LocationOptionVm>>(json) ?? new List<LocationOptionVm>();
                    _memoryCache.Set(districtsCacheKey, districts, TimeSpan.FromHours(6));
                }

                var targetDistrictSlug = districtSlug.ToLowerInvariant();
                district = districts.FirstOrDefault(x => ListingUrl.Slugify(x.Text) == targetDistrictSlug);
                if (district == null)
                    return null;
            }

            var tur = isRent ? "Kiralık" : "Satılık";
            var heading = district != null
                ? $"{district.Text}, {city.Text} {tur} İlanlar"
                : $"{city.Text} {tur} İlanlar";

            var canonicalPath = ListingUrl.ListPath(isRent, city.Text, district?.Text);

            return new ListingLocationVm
            {
                IsRent = isRent,
                CityId = city.Id,
                City = city.Text,
                DistrictId = district?.Id,
                District = district?.Text,
                CanonicalPath = canonicalPath,
                Heading = heading
            };
        }
    }
}
