using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _TheHotPropertyViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _TheHotPropertyViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"]?.Trim();
            if (!string.IsNullOrEmpty(apiUrl) && !apiUrl.EndsWith("/"))
            {
                apiUrl += "/";
            }

            var forSaleList = new List<ResultBestDealDto>();
            var forRentList = new List<ResultBestDealDto>();

            if (!string.IsNullOrEmpty(apiUrl))
            {
                forSaleList = await GetDealsAsync(client, $"{apiUrl}BestDeals?usageType=1");
                forRentList = await GetDealsAsync(client, $"{apiUrl}BestDeals?usageType=2");
            }

            var viewModel = new BestDealsViewModel
            {
                ForSale = forSaleList,
                ForRent = forRentList
            };

            return View(viewModel);
        }

        private async Task<List<ResultBestDealDto>> GetDealsAsync(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ResultBestDealDto>>(json) ?? new List<ResultBestDealDto>();
                }
            }
            catch (Exception)
            {
                // Vitrin yüklenemezse ana sayfa bozulmasın
            }

            return new List<ResultBestDealDto>();
        }
    }
}
