using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class ThemeScaffController : Controller
    {
        public IActionResult ScaffTemplate()
        {
            return View();
        }
    }
}
