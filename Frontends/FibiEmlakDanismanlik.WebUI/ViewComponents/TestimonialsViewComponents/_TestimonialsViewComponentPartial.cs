using FibiEmlakDanismanlik.Dto.FaqDtos;
using FibiEmlakDanismanlik.Dto.TestimonialsDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.TestimonialsViewComponents
{
    public class _TestimonialsViewComponentPartial:ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _TestimonialsViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Testimonails/GetTestimonialsList");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonDAta= await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<List<ResultTestimonialDto>>(jsonDAta);
                return View(value);

            }
            return View(null);
        }
    }
}
