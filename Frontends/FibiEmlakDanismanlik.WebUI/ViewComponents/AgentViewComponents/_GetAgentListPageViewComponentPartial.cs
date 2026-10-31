using FibiEmlakDanismanlik.Dto.AgentDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.AgentViewComponents
{
    public class _GetAgentListPageViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _GetAgentListPageViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue)
        {
            if (!request.Query.TryGetValue(key, out var v)) return defaultValue;
            if (int.TryParse(v.ToString(), out var parsed) && parsed > 0) return parsed;
            return defaultValue;
        }

        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue, int min, int max)
        {
            var val = ReadIntQuery(request, key, defaultValue);
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var page = ReadIntQuery(Request, "page", 1, 1, int.MaxValue);
            var pageSize = ReadIntQuery(Request, "pageSize", 3, 1, 50);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_configuration["Url:ApiUrl"]}Agent/paged?page={page}&pageSize={pageSize}";
            var responseMessage = await client.GetAsync(apiUrl);

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<ResultAgentPagedDto>(jsonData);

                if (pagedResult != null)
                {
                    var vm = new AgentListingPageVm
                    {
                        Items = pagedResult.Items ?? new(),
                        Total = pagedResult.Total,
                        Page = pagedResult.Page,
                        PageSize = pagedResult.PageSize
                    };
                    return View(vm);
                }
            }

            return View(new AgentListingPageVm { Page = page, PageSize = pageSize });
        }
    }
}
