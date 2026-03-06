using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForRentalDetailPageController : Controller
    {
        public IActionResult Index(int id)
        {
            var dto = new ResultAllForRentalListingForPageDto
            {
                ListingId = id
            };

            return View(dto);
        }
    }
}
