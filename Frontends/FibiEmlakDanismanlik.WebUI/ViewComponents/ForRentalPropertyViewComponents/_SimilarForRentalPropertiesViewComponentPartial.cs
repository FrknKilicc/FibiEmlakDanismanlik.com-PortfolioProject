using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForRentalPropertyViewComponents
{
    public class _SimilarForRentalPropertiesViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _SimilarForRentalPropertiesViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}ForRental/GetSimilarForRentalProperties/{id}");

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
