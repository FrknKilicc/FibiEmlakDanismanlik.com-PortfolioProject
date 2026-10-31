using System.Text;

using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _FeaturedPropertViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SiteTextService _siteTextService;

        public _FeaturedPropertViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory, SiteTextService siteTextService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _siteTextService = siteTextService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // İlanlar yüklenemezse ana sayfa bozulmasın
            try
            {
                var apiUrl = _configuration["Url:ApiUrl"];
                var latestCount = (await _siteTextService.GetAsync()).Int("home.latest.count");
                if (string.IsNullOrEmpty(apiUrl) || latestCount <= 0)
                {
                    return View(new List<ResultAllForSaleListinForPageDto>());
                }

                var client = _httpClientFactory.CreateClient();
                var request = new PropertyFilterRequestDto
                {
                    SortBy = "CreatedDate",
                    SortDir = "desc",
                    Page = 1,
                    PageSize = latestCount
                };

                var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var apiBase = apiUrl.TrimEnd('/') + "/";
                var listResp = await client.PostAsync($"{apiBase}ForSales/filter", body);

                if (listResp.IsSuccessStatusCode)
                {
                    var json = await listResp.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<ForSaleFilterResponseDto>(json);
                    var items = responseDto?.Items ?? new List<ResultAllForSaleListinForPageDto>();
                    return View(items);
                }

                return View(new List<ResultAllForSaleListinForPageDto>());
            }
            catch
            {
                return View(new List<ResultAllForSaleListinForPageDto>());
            }
        }
    }
}
