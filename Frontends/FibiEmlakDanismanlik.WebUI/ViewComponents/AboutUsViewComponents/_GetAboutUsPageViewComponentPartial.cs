using FibiEmlakDanismanlik.Dto.AboutUsDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.AboutUsViewComponents
{
    public class _GetAboutUsPageViewComponentPartial:ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _GetAboutUsPageViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}AboutUs/1");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value  = JsonConvert.DeserializeObject<ResultAboutUsDto>(jsonData);
                return View(value);
            }
            return View(null);



        }
    }
}
