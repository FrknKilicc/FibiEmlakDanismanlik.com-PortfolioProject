using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForRentalPageController : Controller
    {
        private readonly ListingLocationResolver _listingLocationResolver;

        public AllForRentalPageController(ListingLocationResolver listingLocationResolver)
        {
            _listingLocationResolver = listingLocationResolver;
        }

        [HttpGet("kiralik")]
        public IActionResult Index()
        {
            return View("Index", (ListingLocationVm?)null);
        }

        [HttpGet("AllForRentalPage")]
        [HttpGet("AllForRentalPage/Index")]
        public IActionResult LegacyIndex()
        {
            return RedirectPermanent("/kiralik" + Request.QueryString);
        }

        [HttpGet("kiralik/{citySlug}")]
        [HttpGet("kiralik/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> Location(string citySlug, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveAsync(true, citySlug, districtSlug);
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
            HttpContext.Items[PageMeta.DescriptionItemKey] = $"{yer} bölgesindeki kiralık konut, arsa ve işyeri ilanları. Fibi Yapı Emlak & Danışmanlık.";

            return View("Index", vm);
        }
    }
}
