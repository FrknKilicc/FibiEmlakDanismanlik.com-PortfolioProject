using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents
{
    public class _PaginationViewComponentPartial:ViewComponent
    {
        

        public IViewComponentResult Invoke()
        {
            return View(); 
        }

    }
}
