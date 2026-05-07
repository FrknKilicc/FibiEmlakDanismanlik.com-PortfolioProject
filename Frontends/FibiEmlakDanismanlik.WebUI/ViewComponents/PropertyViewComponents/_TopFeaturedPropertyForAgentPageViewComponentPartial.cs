using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<IViewComponentResult> InvokeAsync(string status = "forRent")
        {
            status = string.IsNullOrWhiteSpace(status) ? "forRent" : status;
            var client = _httpClientFactory.CreateClient();

            if (status == "forSale")
            {
                var responseMessage = await client.GetAsync(
                    $"{_configuration["Url:ApiUrl"]}ForSales/GetAllForSalesPropertyForListing");

                if (!responseMessage.IsSuccessStatusCode)
                    return View(new List<AgentFeaturedPropertySidebarVm>());

                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAllForSaleListinForPageDto>>(jsonData);

                var top5 = values?
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(5)
                    .Select(x => new AgentFeaturedPropertySidebarVm
                    {
                        ListingId = x.ListingId,
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        CreatedDate = x.CreatedDate,
                        Price = x.Price,
                        PropImgUrl1 = x.PropImgUrl1,
                        PropertyStatusLabel = "Satılık",
                        DetailUrl = $"/AllForSaleDetailPage/Index/{x.ListingId}"
                    })
                    .ToList() ?? new List<AgentFeaturedPropertySidebarVm>();

                ViewBag.SelectedStatus = status;
                return View(top5);
            }
            else
            {
                var responseMessage = await client.GetAsync(
                    $"{_configuration["Url:ApiUrl"]}ForRental/GetAllForRentalPropertyForListing");

                if (!responseMessage.IsSuccessStatusCode)
                    return View(new List<AgentFeaturedPropertySidebarVm>());

                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAllForRentalListingForPageDto>>(jsonData);

                var top5 = values?
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(5)
                    .Select(x => new AgentFeaturedPropertySidebarVm
                    {
                        ListingId = x.ListingId,
                        PropertyName = x.PropertyName,
                        PropertyDescription = x.PropertyDescription,
                        CreatedDate = x.CreatedDate,
                        Price = x.Rent,
                        PropImgUrl1 = x.PropImgUrl1,
                        PropertyStatusLabel = "Kiralık",
                        DetailUrl = $"/AllForRentalDetailPage/Index/{x.ListingId}"
                    })
                    .ToList() ?? new List<AgentFeaturedPropertySidebarVm>();

                ViewBag.SelectedStatus = status;
                return View(top5);
            }
        }
    }
}
