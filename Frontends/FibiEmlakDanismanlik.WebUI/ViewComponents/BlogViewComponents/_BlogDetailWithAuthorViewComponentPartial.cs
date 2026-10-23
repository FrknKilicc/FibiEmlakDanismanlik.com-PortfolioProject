using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _BlogDetailWithAuthorViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _BlogDetailWithAuthorViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetBlogDetailWithAuthorById?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                return View(null);
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultBlogWithAuthorDto>(jsonData);

            if (result != null)
            {
                if (!string.IsNullOrWhiteSpace(result.BlogTitle))
                {
                    HttpContext.Items[PageMeta.TitleItemKey] = result.BlogTitle;
                }

                var desc = PageMeta.StripHtmlAndTruncate(result.BlogDescription, PageMeta.DescriptionMaxLength);
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    HttpContext.Items[PageMeta.DescriptionItemKey] = desc;
                }
            }

            return View(result);
        }
    }
}
