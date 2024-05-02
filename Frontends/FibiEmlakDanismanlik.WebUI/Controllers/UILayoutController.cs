using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class UILayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
