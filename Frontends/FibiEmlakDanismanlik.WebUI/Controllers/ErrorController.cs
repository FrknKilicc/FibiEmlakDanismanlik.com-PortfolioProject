using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode:int}")]
        public IActionResult Status(int statusCode)
        {
            ViewData["Robots"] = "noindex";
            if (statusCode == 404)
                return View("NotFound");
            return View("General", statusCode);
        }
    }
}
