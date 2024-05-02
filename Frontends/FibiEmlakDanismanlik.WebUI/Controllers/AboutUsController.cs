using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
