using FibiEmlakDanismanlik.Dto.BlogDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _GetLast3BlogWithAuthorViewComponentPartial: ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _GetLast3BlogWithAuthorViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetLast3BlogWithAuthor");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<ResultBlogWithAuthorDto>>(jsonData);
                    return View(viewName, result ?? new List<ResultBlogWithAuthorDto>());
                }
            }
            catch (Exception)
            {
                // Bloglar yüklenemezse sayfa bozulmasın
            }

            return View(viewName, new List<ResultBlogWithAuthorDto>());
        }
    }
}
