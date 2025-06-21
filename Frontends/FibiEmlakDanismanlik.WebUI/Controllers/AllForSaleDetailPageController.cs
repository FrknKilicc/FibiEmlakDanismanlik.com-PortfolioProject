using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForSaleDetailPageController : Controller
    {
        public IActionResult Index(int id)
        {
            var dto = new ResultAllForSaleListinForPageDto
            {
                ListingId = id
            };
            return View(dto);
        }
    }
}
