using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.HomeSectionImageDtos;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _MostPopularPlacesViewComponentPartial : ViewComponent
    {
        private const string SectionKey = "PopularPlaces";

        private const string DefaultPrimaryImageUrl = "/Realshed/assets/images/resource/popular-place-1.jpg";
        private const string DefaultPrimaryImageAlt = "Fibi Emlak Danışmanlık merkez ofis";

        private const string DefaultSecondaryImageUrl = "/Realshed/assets/images/resource/popular-place-2.jpg";
        private const string DefaultSecondaryImageAlt = "Fibi Emlak Danışmanlık ofis içi";

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
            var sectionImages = new List<ResultHomeSectionImageDto>();

            if (!string.IsNullOrEmpty(apiUrl))
            {
                var forSaleTask = GetPlacesAsync(client, $"{apiUrl}PopularPlaces?usageType=1");
                var forRentTask = GetPlacesAsync(client, $"{apiUrl}PopularPlaces?usageType=2");
                var imagesTask = GetSectionImagesAsync(client, $"{apiUrl}HomeSectionImages?sectionKey={SectionKey}");

                await Task.WhenAll(forSaleTask, forRentTask, imagesTask);

                forSale = await forSaleTask;
                forRent = await forRentTask;
                sectionImages = await imagesTask;
            }

            var slot1Image = sectionImages.FirstOrDefault(x => x.Slot == 1 && x.IsActive && !string.IsNullOrWhiteSpace(x.ImageUrl));
            var slot2Image = sectionImages.FirstOrDefault(x => x.Slot == 2 && x.IsActive && !string.IsNullOrWhiteSpace(x.ImageUrl));

            var primaryImageUrl = !string.IsNullOrWhiteSpace(slot1Image?.ImageUrl)
                ? slot1Image.ImageUrl
                : DefaultPrimaryImageUrl;

            var primaryImageAlt = !string.IsNullOrWhiteSpace(slot1Image?.AltText)
                ? slot1Image.AltText
                : DefaultPrimaryImageAlt;

            var secondaryImageUrl = !string.IsNullOrWhiteSpace(slot2Image?.ImageUrl)
                ? slot2Image.ImageUrl
                : DefaultSecondaryImageUrl;

            var secondaryImageAlt = !string.IsNullOrWhiteSpace(slot2Image?.AltText)
                ? slot2Image.AltText
                : DefaultSecondaryImageAlt;

            var viewModel = new PopularPlacesViewModel
            {
                ForSale = forSale,
                ForRent = forRent,
                PrimaryImageUrl = primaryImageUrl,
                PrimaryImageAlt = primaryImageAlt,
                SecondaryImageUrl = secondaryImageUrl,
                SecondaryImageAlt = secondaryImageAlt
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

        private async Task<List<ResultHomeSectionImageDto>> GetSectionImagesAsync(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ResultHomeSectionImageDto>>(json) ?? new List<ResultHomeSectionImageDto>();
                }
            }
            catch (Exception)
            {
                // Görseller yüklenemezse ana sayfa bozulmasın
            }

            return new List<ResultHomeSectionImageDto>();
        }
    }
}
