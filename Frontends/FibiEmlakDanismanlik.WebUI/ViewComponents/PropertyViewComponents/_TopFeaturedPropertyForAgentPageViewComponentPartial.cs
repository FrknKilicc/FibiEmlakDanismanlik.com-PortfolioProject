using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _TopFeaturedPropertyForAgentPageViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _TopFeaturedPropertyForAgentPageViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
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

            var forSaleTask = !string.IsNullOrEmpty(apiUrl)
                ? GetDealsAsync(client, $"{apiUrl}BestDeals?usageType=1")
                : Task.FromResult(new List<ResultBestDealDto>());

            var forRentTask = !string.IsNullOrEmpty(apiUrl)
                ? GetDealsAsync(client, $"{apiUrl}BestDeals?usageType=2")
                : Task.FromResult(new List<ResultBestDealDto>());

            await Task.WhenAll(forSaleTask, forRentTask);

            var forSaleList = await forSaleTask;
            var forRentList = await forRentTask;

            var list = forSaleList
                .Concat(forRentList)
                .OrderByDescending(x => x.CreatedDate)
                .Take(6)
                .Select(x => new AgentFeaturedPropertySidebarVm
                {
                    ListingId = x.ListingId,
                    PropertyName = x.PropertyName ?? "",
                    PropertyDescription = x.PropertyDescription ?? "",
                    CreatedDate = x.CreatedDate,
                    Price = x.Price,
                    PropImgUrl1 = x.ImageUrl,
                    PropertyStatusLabel = x.UsageType == 2 ? "Kiralık" : "Satılık",
                    DetailUrl = (x.UsageType == 2
                        ? Url.Action("Index", "AllForRentalDetailPage", new { id = x.ListingId, listingTypeId = x.ListingTypeId })
                        : Url.Action("Index", "AllForSaleDetailPage", new { id = x.ListingId, listingTypeId = x.ListingTypeId })) ?? "#"
                })
                .ToList();

            return View(list);
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
                // Hata durumunda boş liste döner
            }

            return new List<ResultBestDealDto>();
        }
    }
}
