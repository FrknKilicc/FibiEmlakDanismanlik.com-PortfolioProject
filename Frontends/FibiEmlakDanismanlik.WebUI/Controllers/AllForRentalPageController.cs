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
        public async Task<IActionResult> Index()
        {
            if (Request.Query.ContainsKey("cityId") &&
                int.TryParse(Request.Query["cityId"], out var cityId) &&
                cityId > 0)
            {
                var allowedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "cityId", "city", "districtId", "district", "page"
                };

                var onlyAllowedKeys = Request.Query.Keys.All(k => allowedKeys.Contains(k));
                var pageVal = Request.Query["page"].ToString();
                var validPage = string.IsNullOrEmpty(pageVal) || pageVal == "1";

                if (onlyAllowedKeys && validPage)
                {
                    int? districtId = null;
                    var hasDistrictId = Request.Query.ContainsKey("districtId") && !string.IsNullOrWhiteSpace(Request.Query["districtId"]);
                    if (hasDistrictId)
                    {
                        if (int.TryParse(Request.Query["districtId"], out var parsedDistrictId) && parsedDistrictId > 0)
                        {
                            districtId = parsedDistrictId;
                        }
                        else
                        {
                            return View("Index", (ListingLocationVm?)null);
                        }
                    }

                    var names = await _listingLocationResolver.FindNamesByIdsAsync(cityId, districtId);
                    if (names.HasValue)
                    {
                        return RedirectPermanent(ListingUrl.ListPath(true, names.Value.City, names.Value.District));
                    }
                }
            }

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
