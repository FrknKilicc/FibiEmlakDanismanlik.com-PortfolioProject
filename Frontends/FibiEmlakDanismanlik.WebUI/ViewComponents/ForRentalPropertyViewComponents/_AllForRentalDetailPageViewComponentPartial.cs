using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
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
        public async Task<IViewComponentResult> InvokeAsync(int id, int? listingTypeId = null)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Url:ApiUrl"]}ForRental/GetUnifiedForRentalPropertyById/{id}";
            if (listingTypeId.HasValue)
            {
                url += $"?listingTypeId={listingTypeId.Value}";
            }
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ResultAllForRentalListingForPageDto>(jsonData);

                if (value == null)
                    return View("Error");

                value.ListingId = id;

                if (string.IsNullOrWhiteSpace(value.ListingType))
                    value.ListingType = value.ListingTypeName;

                if (!string.IsNullOrWhiteSpace(value.PropertyName))
                {
                    HttpContext.Items[PageMeta.TitleItemKey] = value.PropertyName;
                }
                var desc = PageMeta.StripHtmlAndTruncate(value.PropertyDescription, PageMeta.DescriptionMaxLength);
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    HttpContext.Items[PageMeta.DescriptionItemKey] = desc;
                }

                ViewBag.ApiUrl = _configuration["Url:ApiUrl"];
                return View(value);
            }

            return View("Error");
        }
    }
 }
