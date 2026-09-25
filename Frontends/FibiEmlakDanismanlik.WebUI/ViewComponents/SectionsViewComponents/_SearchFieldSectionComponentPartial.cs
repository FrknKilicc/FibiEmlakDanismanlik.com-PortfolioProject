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
            var apiUrl = _configuration["Url:ApiUrl"] ?? "https://localhost:7015/api/";
            ViewBag.ApiUrl = apiUrl;
            return View();
        }
    }
}

