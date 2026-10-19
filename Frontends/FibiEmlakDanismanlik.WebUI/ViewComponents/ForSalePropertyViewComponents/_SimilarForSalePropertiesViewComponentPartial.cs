using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    public class _SimilarForSalePropertiesViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _SimilarForSalePropertiesViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}ForSales/GetSimilarForSaleProperties/{id}");

                if (!response.IsSuccessStatusCode)
                    return View(new List<SimilarPropertyItemDto>());

                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<SimilarPropertyItemDto>>(jsonData);

                return View(values ?? new List<SimilarPropertyItemDto>());
            }
            catch
            {
                return View(new List<SimilarPropertyItemDto>());
            }
        }
    }
}
