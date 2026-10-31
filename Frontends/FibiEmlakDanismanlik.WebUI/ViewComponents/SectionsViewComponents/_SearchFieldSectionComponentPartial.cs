using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.SectionsViewComponents
{
    public class _SearchFieldSectionComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public _SearchFieldSectionComponentPartial(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.ApiUrl = _configuration.GetBrowserApiUrl() ?? "https://localhost:7015/api/";
            return View();
        }
    }
}

