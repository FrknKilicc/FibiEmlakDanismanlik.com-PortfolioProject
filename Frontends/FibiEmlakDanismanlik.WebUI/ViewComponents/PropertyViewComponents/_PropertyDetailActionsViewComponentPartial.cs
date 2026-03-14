using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _PropertyDetailActionsViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke(CompareActionButtonVm model)
        {
            return View(model);
        }
    }
}
