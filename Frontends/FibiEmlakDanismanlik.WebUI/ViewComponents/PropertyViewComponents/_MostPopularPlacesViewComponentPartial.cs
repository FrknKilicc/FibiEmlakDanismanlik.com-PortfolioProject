using System;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _MostPopularPlacesViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _MostPopularPlacesViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

            var forSale = new ResultPopularPlaceDto();
            var forRent = new ResultPopularPlaceDto();

            if (!string.IsNullOrEmpty(apiUrl))
            {
                var forSaleTask = GetPlacesAsync(client, $"{apiUrl}PopularPlaces?usageType=1");
                var forRentTask = GetPlacesAsync(client, $"{apiUrl}PopularPlaces?usageType=2");

                await Task.WhenAll(forSaleTask, forRentTask);

                forSale = await forSaleTask;
                forRent = await forRentTask;
            }

            var viewModel = new PopularPlacesViewModel
            {
                ForSale = forSale,
                ForRent = forRent
            };

            return View(viewModel);
        }

        private async Task<ResultPopularPlaceDto> GetPlacesAsync(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResultPopularPlaceDto>(json) ?? new ResultPopularPlaceDto();
                }
            }
            catch (Exception)
            {
                // Popüler konumlar yüklenemezse ana sayfa bozulmasın
            }

            return new ResultPopularPlaceDto();
        }
    }
}
