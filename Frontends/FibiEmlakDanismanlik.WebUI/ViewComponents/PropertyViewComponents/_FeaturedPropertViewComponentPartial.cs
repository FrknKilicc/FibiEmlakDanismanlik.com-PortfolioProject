using System.Text;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _FeaturedPropertViewComponentPartial : ViewComponent
    {
        private const int LatestCount = 10;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _FeaturedPropertViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // İlanlar yüklenemezse ana sayfa bozulmasın
            try
            {
                var apiUrl = _configuration["Url:ApiUrl"];
                if (string.IsNullOrEmpty(apiUrl))
                {
                    return View(new List<ResultAllForSaleListinForPageDto>());
                }

                var client = _httpClientFactory.CreateClient();
                var request = new PropertyFilterRequest
                {
                    SortBy = "CreatedDate",
                    SortDir = "desc",
                    Page = 1,
                    PageSize = LatestCount
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
