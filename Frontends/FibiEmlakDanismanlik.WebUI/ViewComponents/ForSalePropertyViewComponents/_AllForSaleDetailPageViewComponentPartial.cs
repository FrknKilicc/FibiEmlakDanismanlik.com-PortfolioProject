using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    public class _AllForSaleDetailPageViewComponentPartial:ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _AllForSaleDetailPageViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}ForSales/GetUnifiedForSalesPropertyById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ResultAllForSaleListinForPageDto>(jsonData);

                return View(value);
            }
            else if(response==null || response.StatusCode.Equals(500))
            {
                 return View(response.StatusCode);
            }
            return null;

        }
    }
}
