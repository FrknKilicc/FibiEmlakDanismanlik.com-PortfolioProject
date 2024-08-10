using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewComponentHelper _viewComponentHelper;

        public HomeController(ILogger<HomeController> logger, IViewComponentHelper viewComponentHelper)
        {
            _logger = logger;
            _viewComponentHelper = viewComponentHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Get10RecentRentalProperties()
        {
            return ViewComponent("_FeaturedRentalPropertViewComponentPartial");
        }


    }
}
