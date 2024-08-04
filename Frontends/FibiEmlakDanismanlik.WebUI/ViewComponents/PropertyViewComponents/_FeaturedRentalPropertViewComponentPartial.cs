using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _FeaturedRentalPropertViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _FeaturedRentalPropertViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Rental");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultRentalCommercialPropertyListingDto>>(jsonData);
                return View(value);
            }
            else
            {
                return View(null);
            }

        }
    }
}
