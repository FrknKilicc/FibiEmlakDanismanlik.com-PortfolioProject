using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    public class _AllForSaleDetailPageViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public _AllForSaleDetailPageViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(ResultAllForSaleListinForPageDto value)
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
            }

            ViewBag.ApiUrl = _configuration["Url:ApiUrl"];

            var layout = await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache);
            ViewBag.OfficeAddress = layout.Contact?.OfficeAddress;

            return View(value);
        }
    }
}
