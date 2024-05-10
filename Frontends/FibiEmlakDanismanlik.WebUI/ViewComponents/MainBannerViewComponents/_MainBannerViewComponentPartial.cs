using FibiEmlakDanismanlik.Dto.MainBannerDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.MainBannerViewComponents
{
    public class _MainBannerViewComponentPartial:ViewComponent
    {
        IHttpClientFactory _httpClientFactory;

        public _MainBannerViewComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult>  InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7015/api/MainBanner");
            if (responseMessage.IsSuccessStatusCode)
            {
                var JsonData=await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultMainBannerDto>>(JsonData);
                return View(value); 
            }
            else
            {
                return View(null);
            }
        }
    }
}
