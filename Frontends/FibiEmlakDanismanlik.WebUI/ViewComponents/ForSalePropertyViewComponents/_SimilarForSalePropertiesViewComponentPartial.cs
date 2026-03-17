using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    public class _SimilarForSalePropertiesViewComponentPartial:ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _SimilarForSalePropertiesViewComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7015/api/ForSales/GetSimilarForSaleProperties/{id}");

            if (!response.IsSuccessStatusCode)
                return View(new List<SimilarPropertyItemDto>());

            var jsonData = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<SimilarPropertyItemDto>>(jsonData);

            return View(values ?? new List<SimilarPropertyItemDto>());
        }
}
}
