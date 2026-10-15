using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.HomeSectionImageDtos;
using FibiEmlakDanismanlik.Dto.TestimonialsDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.TestimonialsViewComponents
{
    public class _HomeTestimonialsViewComponentPartial : ViewComponent
    {
        private const string SectionKey = "Testimonials";
        private const string DefaultBackgroundImageUrl = "/Realshed/assets/images/background/testimonial-home.jpg";
        private const string DefaultBackgroundImageAlt = "Yeni evinin önünde mutlu aile";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _HomeTestimonialsViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

            var testimonials = new List<ResultTestimonialDto>();
            var sectionImages = new List<ResultHomeSectionImageDto>();

            if (!string.IsNullOrEmpty(apiUrl))
            {
                var testimonialsTask = GetTestimonialsAsync(client, $"{apiUrl}Testimonails/GetTestimonialsList");
                var imagesTask = GetSectionImagesAsync(client, $"{apiUrl}HomeSectionImages?sectionKey={SectionKey}");

                await Task.WhenAll(testimonialsTask, imagesTask);

                testimonials = (await testimonialsTask)
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Testimonail))
                    .OrderBy(x => x.Id)
                    .ToList();

                sectionImages = await imagesTask;
            }

            var slot1Image = sectionImages.FirstOrDefault(x => x.Slot == 1 && x.IsActive && !string.IsNullOrWhiteSpace(x.ImageUrl));

            var backgroundImageUrl = !string.IsNullOrWhiteSpace(slot1Image?.ImageUrl)
                ? slot1Image.ImageUrl
                : DefaultBackgroundImageUrl;

            var backgroundImageAlt = !string.IsNullOrWhiteSpace(slot1Image?.AltText)
                ? slot1Image.AltText
                : DefaultBackgroundImageAlt;

            var viewModel = new HomeTestimonialsViewModel
            {
                Testimonials = testimonials,
                BackgroundImageUrl = backgroundImageUrl,
                BackgroundImageAlt = backgroundImageAlt
            };

            return View(viewModel);
        }

        private async Task<List<ResultTestimonialDto>> GetTestimonialsAsync(HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ResultTestimonialDto>>(json) ?? new List<ResultTestimonialDto>();
                }
            }
            catch (Exception)
            {
                // Yorumlar yüklenemezse ana sayfa bozulmasın
            }

            return new List<ResultTestimonialDto>();
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
