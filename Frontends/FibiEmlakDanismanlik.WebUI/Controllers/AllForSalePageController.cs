using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForSalePageController : Controller
    {
        private readonly ListingLocationResolver _listingLocationResolver;

        public AllForSalePageController(ListingLocationResolver listingLocationResolver)
        {
            _listingLocationResolver = listingLocationResolver;
        }

        [HttpGet("satilik")]
        public IActionResult Index()
        {
            return View("Index", (ListingLocationVm?)null);
        }

        [HttpGet("AllForSalePage")]
        [HttpGet("AllForSalePage/Index")]
        public IActionResult LegacyIndex()
        {
            return RedirectPermanent("/satilik" + Request.QueryString);
        }

        [HttpGet("satilik/{citySlug}")]
        [HttpGet("satilik/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> Location(string citySlug, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveAsync(false, citySlug, districtSlug);
            if (vm == null)
            {
                return NotFound();
            }

            if (!string.Equals(Request.Path.Value, vm.CanonicalPath, StringComparison.Ordinal))
            {
                return RedirectPermanent(vm.CanonicalPath + Request.QueryString);
            }

            HttpContext.Items[PageMeta.TitleItemKey] = vm.Heading;
            var yer = !string.IsNullOrWhiteSpace(vm.District) ? $"{vm.District}, {vm.City}" : vm.City;
            HttpContext.Items[PageMeta.DescriptionItemKey] = $"{yer} bölgesindeki satılık konut, arsa ve işyeri ilanları. Fibi Yapı Emlak & Danışmanlık.";

            return View("Index", vm);
        }
    }
}
