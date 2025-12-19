using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Blog/Detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            ViewBag.BlogId = id;    
            return View();

        }

    }
}
