using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForSaleDetailPageController : Controller
    {
        public IActionResult Index(int id, int? listingTypeId)
        {
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = id,
                ListingTypeId = listingTypeId ?? 0
            };
            return View(dto);
        }
    }
}
