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

        private IActionResult RenderLanding(ListingLocationVm? vm)
        {
            return ListingLandingResult.Render(this, vm, true, "Index");
        }

        [HttpGet("kiralik/{citySlug}")]
        [HttpGet("kiralik/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> Location(string citySlug, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveAsync(true, citySlug, districtSlug);
            return RenderLanding(vm);
        }

        [HttpGet("kiralik/{category:listingcategory}")]
        [HttpGet("kiralik/{category:listingcategory}/{citySlug}/{districtSlug}")]
        public async Task<IActionResult> CategoryLocation(string category, string? citySlug = null, string? districtSlug = null)
        {
            var vm = await _listingLocationResolver.ResolveCategoryAsync(true, category, citySlug, districtSlug);
            return RenderLanding(vm);
        }
    }
}
