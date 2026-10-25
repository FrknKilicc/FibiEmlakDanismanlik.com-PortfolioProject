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

        private IActionResult RenderLanding(ListingLocationVm? vm)
        {
            return ListingLandingResult.Render(this, vm, false, "Index");
        }

        [HttpGet("satilik/{citySlug}")]
        [HttpGet("satilik/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> Location(string citySlug, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveAsync(false, citySlug, districtSlug);
            return RenderLanding(vm);
        }

        [HttpGet("satilik/{category:listingcategory}")]
        [HttpGet("satilik/{category:listingcategory}/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> CategoryLocation(string category, string? citySlug = null, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveCategoryAsync(false, category, citySlug, districtSlug);
            return RenderLanding(vm);
        }
    }
}
