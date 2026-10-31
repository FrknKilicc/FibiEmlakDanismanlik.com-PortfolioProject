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
                        return RedirectPermanent(ListingUrl.ListPath(false, names.Value.City, names.Value.District));
                    }
                }
            }

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
