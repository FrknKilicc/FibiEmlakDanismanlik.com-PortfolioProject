using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForRentalPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
