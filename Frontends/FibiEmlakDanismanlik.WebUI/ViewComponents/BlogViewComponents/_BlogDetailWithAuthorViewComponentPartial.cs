using FibiEmlakDanismanlik.Dto.BlogDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _BlogDetailWithAuthorViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _BlogDetailWithAuthorViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetBlogDetailWithAuthorById?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(null);

            }
            var jsonData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultBlogWithAuthorDto>(jsonData);
            return View(result);
        }
    }
}
