using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Models.FibiServices;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    /// <summary>
    /// FİBİ Hizmetler modüllerinin ortak controller yapısı.
    /// </summary>
    public class FibiServicesController : Controller
    {
        private readonly IConfiguration _configuration;

        public FibiServicesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private FibiServicePageVm BuildPage(string key)
        {
            return new FibiServicePageVm
            {
                Active = FibiServiceCatalog.Get(key),
                Items = FibiServiceCatalog.Items,
                ApiUrl = _configuration.GetBrowserApiUrl() ?? string.Empty
            };
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(RiskAnalysis));
        }

        public IActionResult RiskAnalysis()
        {
            return View(BuildPage(FibiServiceCatalog.RiskAnalysis));
        }

        public IActionResult NeighborhoodAnalysis()
        {
            return View(BuildPage(FibiServiceCatalog.NeighborhoodAnalysis));
        }

        public IActionResult Finance()
        {
            return View("ComingSoon", BuildPage(FibiServiceCatalog.Finance));
        }

        public IActionResult Expert()
        {
            return View("ComingSoon", BuildPage(FibiServiceCatalog.Expert));
        }

        public IActionResult Investment()
        {
            return View("ComingSoon", BuildPage(FibiServiceCatalog.Investment));
        }
    }
}
