using FibiEmlakDanismanlik.Dto.SocialMediaDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.SocialMediaViewComponents
{
    public class _SocialMediaViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _SocialMediaViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();

            var responseMessage = await client.GetAsync(
                $"{_configuration["Url:ApiUrl"]}SocialMedia");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                var result = JsonConvert
                    .DeserializeObject<List<ResultSocialMediaDto>>(jsonData);

                return View(result);
            }
            return View(new List<ResultSocialMediaDto>());
        }
    }
}
