using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public static class ListingLandingResult
    {
        public static IActionResult Render(Controller controller, ListingLocationVm? vm, bool isRent, string viewPath)
        {
            if (vm == null)
            {
                return controller.NotFound();
            }

            if (!string.Equals(controller.Request.Path.Value, vm.CanonicalPath, StringComparison.Ordinal))
            {
                return controller.RedirectPermanent(vm.CanonicalPath + controller.Request.QueryString);
            }

            controller.HttpContext.Items[PageMeta.TitleItemKey] = vm.Heading;

            var turLower = isRent ? "kiralık" : "satılık";
            var turUpper = isRent ? "Kiralık" : "Satılık";
            var yer = !string.IsNullOrWhiteSpace(vm.District) ? $"{vm.District}, {vm.City}" : vm.City;

            string description;
            if (!string.IsNullOrWhiteSpace(vm.Category))
            {
                var catLower = vm.Category.ToLowerInvariant().Trim() switch
                {
                    "konut" => "konut",
                    "arsa" => "arsa",
                    "isyeri" => "işyeri",
                    var other => other
                };
                description = !string.IsNullOrWhiteSpace(yer)
                    ? $"{yer} bölgesindeki {turLower} {catLower} ilanları. Fibi Yapı Emlak & Danışmanlık."
                    : $"{turUpper} {catLower} ilanları. Fibi Yapı Emlak & Danışmanlık.";
            }
            else
            {
                description = $"{yer} bölgesindeki {turLower} konut, arsa ve işyeri ilanları. Fibi Yapı Emlak & Danışmanlık.";
            }

            controller.HttpContext.Items[PageMeta.DescriptionItemKey] = description;

            return controller.View(viewPath, vm);
        }
    }
}
