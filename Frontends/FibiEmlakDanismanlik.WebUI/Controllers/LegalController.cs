using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    /// <summary>
    /// Kullanım koşulları ve gizlilik politikası gibi yasal bilgilendirme sayfalarını yöneten kontrolcü.
    /// </summary>
    public class LegalController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public LegalController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Kullanım Koşulları sayfasını görüntüler.
        /// </summary>
        [HttpGet("kullanim-kosullari")]
        public async Task<IActionResult> Terms()
        {
            HttpContext.Items[PageMeta.TitleItemKey] = "Kullanım Koşulları";
            HttpContext.Items[PageMeta.DescriptionItemKey] = "Fibi Yapı Emlak & Danışmanlık web sitesi kullanım koşulları, ilan ve analiz araçları şartları ile yasal bilgilendirmeler.";

            var footerViewModel = await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache);
            return View(footerViewModel);
        }

        /// <summary>
        /// Gizlilik Politikası sayfasını görüntüler.
        /// </summary>
        [HttpGet("gizlilik-politikasi")]
        public async Task<IActionResult> Privacy()
        {
            HttpContext.Items[PageMeta.TitleItemKey] = "Gizlilik Politikası";
            HttpContext.Items[PageMeta.DescriptionItemKey] = "Fibi Yapı Emlak & Danışmanlık gizlilik politikası, 6698 sayılı KVKK kapsamında kişisel verilerin işlenmesi ve korunması.";

            var footerViewModel = await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache);
            return View(footerViewModel);
        }

        /// <summary>
        /// KVKK Aydınlatma Metni sayfasını görüntüler.
        /// </summary>
        [HttpGet("kvkk-aydinlatma-metni")]
        public async Task<IActionResult> Kvkk()
        {
            HttpContext.Items[PageMeta.TitleItemKey] = "KVKK Aydınlatma Metni";
            HttpContext.Items[PageMeta.DescriptionItemKey] = "Fibi Yapı Emlak & Danışmanlık 6698 sayılı KVKK kapsamında kişisel verilerin işlenmesine ilişkin aydınlatma metni.";

            var footerViewModel = await FooterContentLoader.LoadAsync(_httpClientFactory, _configuration, _memoryCache);
            return View(footerViewModel);
        }
    }
}
