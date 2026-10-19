using FibiEmlakDanismanlik.WebUI.Models.ListingsMap;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    /// <summary>
    /// Haritada İlanlar sayfası controller yapısı.
    /// Satılık ve kiralık tüm ilanların tek harita üzerinde filtrelenerek görüntülenmesini sağlar.
    /// </summary>
    public class ListingsMapController : Controller
    {
        private readonly IConfiguration _configuration;

        public ListingsMapController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Haritada ilanlar sayfasını görüntüler.
        /// URL parametrelerinden gelen filtreleri denetler ve görünüm modeline aktarır.
        /// </summary>
        /// <param name="usage">Kullanım türü filtresi ("sale" veya "rent")</param>
        /// <param name="category">Kategori filtresi ("housing", "land" veya "commercial")</param>
        public IActionResult Index(string? usage, string? category)
        {
            var normalizedUsage = (usage ?? string.Empty).Trim().ToLowerInvariant();
            if (normalizedUsage != "sale" && normalizedUsage != "rent")
            {
                normalizedUsage = string.Empty;
            }

            var normalizedCategory = (category ?? string.Empty).Trim().ToLowerInvariant();
            if (normalizedCategory != "housing" && normalizedCategory != "land" && normalizedCategory != "commercial")
            {
                normalizedCategory = string.Empty;
            }

            var vm = new ListingsMapPageVm
            {
                ApiUrl = _configuration["Url:ApiUrl"] ?? string.Empty,
                InitialUsage = normalizedUsage,
                InitialCategory = normalizedCategory
            };

            return View(vm);
        }
    }
}
