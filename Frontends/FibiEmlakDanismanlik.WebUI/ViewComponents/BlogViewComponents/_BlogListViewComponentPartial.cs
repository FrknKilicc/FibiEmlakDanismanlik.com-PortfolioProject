using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _BlogListViewComponentPartial: ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _BlogListViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue)
        {
            if (!request.Query.TryGetValue(key, out var v)) return defaultValue;
            if (int.TryParse(v.ToString(), out var parsed) && parsed > 0) return parsed;
            return defaultValue;
        }

        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue, int min, int max)
        {
            var val = ReadIntQuery(request, key, defaultValue);
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var page = ReadIntQuery(Request, "page", 1, 1, int.MaxValue);
            var pageSize = ReadIntQuery(Request, "pageSize", 3, 1, 100);

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetBlogListWithAuthor");

            if (!responseMessage.IsSuccessStatusCode)
            {
                return View(new BlogListingPageVm());
            }

            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var allBlogs = JsonConvert.DeserializeObject<List<ResultBlogWithAuthorDto>>(jsonData) ?? new List<ResultBlogWithAuthorDto>();

            allBlogs = allBlogs
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            var total = allBlogs.Count;

            var items = allBlogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new BlogListingPageVm
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return View(vm);
        }
    }
}
