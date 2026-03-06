using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForRentalPropertyViewComponents
{
    public class _AllForRentalDetailPageViewComponentPartial:ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _AllForRentalDetailPageViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}ForRental/GetUnifiedForRentalPropertyById/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ResultAllForRentalListingForPageDto>(jsonData);

                if (value == null)
                    return View("Error");

                value.ListingId = id;

                if (string.IsNullOrWhiteSpace(value.ListingType))
                    value.ListingType = value.ListingTypeName;

                ViewBag.ApiUrl = _configuration["Url:ApiUrl"];
                return View(value);
            }

            return View("Error");
        }
    }
 }
