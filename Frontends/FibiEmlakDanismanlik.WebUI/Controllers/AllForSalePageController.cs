using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForSalePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
