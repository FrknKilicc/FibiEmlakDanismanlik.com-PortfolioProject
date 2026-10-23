using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForRentalPropertyViewComponents
{
    public class _AllForRentalDetailPageViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public _AllForRentalDetailPageViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(ResultAllForRentalListingForPageDto value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(value.PropertyName))
                {
                    HttpContext.Items[PageMeta.TitleItemKey] = value.PropertyName;
                }
                var desc = PageMeta.StripHtmlAndTruncate(value.PropertyDescription, PageMeta.DescriptionMaxLength);
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    HttpContext.Items[PageMeta.DescriptionItemKey] = desc;
                }

                var firstImage = new[]
                {
                    value.PropImgUrl1, value.PropImgUrl2, value.PropImgUrl3, value.PropImgUrl4,
                    value.PropImgUrl5, value.PropImgUrl6, value.PropImgUrl7, value.PropImgUrl8,
                    value.PropImgUrl9, value.PropImgUrl10,
                    value.PropImgUrl11, value.PropImgUrl12, value.PropImgUrl13, value.PropImgUrl14, value.PropImgUrl15,
                    value.PropImgUrl16, value.PropImgUrl17, value.PropImgUrl18, value.PropImgUrl19, value.PropImgUrl20,
                    value.PropImgUrl21, value.PropImgUrl22, value.PropImgUrl23, value.PropImgUrl24, value.PropImgUrl25,
                    value.PropImgUrl26, value.PropImgUrl27, value.PropImgUrl28, value.PropImgUrl29, value.PropImgUrl30
                }.FirstOrDefault(img => !string.IsNullOrWhiteSpace(img) && !string.Equals(img.Trim(), "string", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(firstImage))
                {
                    var trimmed = firstImage.Trim();
                    if (!trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                        !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!trimmed.StartsWith('/'))
                        {
                            trimmed = "/" + trimmed;
                        }
                    }
                    HttpContext.Items[PageMeta.ImageItemKey] = trimmed;
                }
            }

            ViewBag.ApiUrl = _configuration["Url:ApiUrl"];

            var layout = await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache);
            ViewBag.OfficeAddress = layout.Contact?.OfficeAddress;

            return View(value);
        }
    }
}
