using FibiEmlakDanismanlik.Dto.BlogDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _BlogCategoriesViewComponentPartial:ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _BlogCategoriesViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetTopBlogCategories?take=5");

            if (!responseMessage.IsSuccessStatusCode)
            {
                return View(new List<BlogCategoryCountDto>());
            }

            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<BlogCategoryCountDto>>(jsonData) ?? new List<BlogCategoryCountDto>();

            return View(values);
        }
    }
}
