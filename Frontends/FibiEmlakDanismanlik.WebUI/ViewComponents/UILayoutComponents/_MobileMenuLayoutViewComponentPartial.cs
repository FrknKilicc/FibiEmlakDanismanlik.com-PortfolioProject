using FibiEmlakDanismanlik.WebUI.Models.Footer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents
{
    public class _MobileMenuLayoutViewComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public _MobileMenuLayoutViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache));
        }
    }
}
