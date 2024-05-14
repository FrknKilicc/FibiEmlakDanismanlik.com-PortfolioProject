using FibiEmlakDanismanlik.Dto.MainBannerDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http; 

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.MainBannerViewComponents
{
    public class _MainBannerViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration; 
        private readonly IHttpClientFactory _httpClientFactory;

        public _MainBannerViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}MainBanner"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultMainBannerDto>>(jsonData);
                return View(value);
            }
            else
            {
                return View(null);
            }
        }
    }
}
