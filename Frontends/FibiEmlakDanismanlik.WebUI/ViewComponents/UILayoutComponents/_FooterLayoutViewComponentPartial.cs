using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents
{
    public class _FooterLayoutViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
