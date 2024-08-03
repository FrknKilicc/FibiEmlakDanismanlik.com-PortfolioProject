using FibiEmlakDanismanlik.Dto.MainBannerDtos;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _FeaturedPropertViewComponentPartial:ViewComponent
    {
       private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _FeaturedPropertViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}ForSales");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultAllForSaleListingDto>>(jsonData);
                return View(value);
            }
            else
            {
                return View(null);
            }
            
        }
    }
}
