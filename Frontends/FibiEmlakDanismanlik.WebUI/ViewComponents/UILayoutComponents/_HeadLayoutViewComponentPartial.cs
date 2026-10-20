using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.UILayoutComponents
{
    public class _HeadLayoutViewComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var rawTitle = (HttpContext.Items[PageMeta.TitleItemKey] as string)
                           ?? (ViewData["Title"] as string);

            var title = !string.IsNullOrWhiteSpace(rawTitle)
                ? $"{rawTitle.Trim()} | {PageMeta.SiteName}"
                : PageMeta.SiteName;

            var rawDescription = (HttpContext.Items[PageMeta.DescriptionItemKey] as string)
                                 ?? (ViewData["Description"] as string);

            var description = !string.IsNullOrWhiteSpace(rawDescription)
                ? rawDescription.Trim()
                : PageMeta.DefaultDescription;

            return View(new PageMetaVm
            {
                Title = title,
                Description = description
            });
        }
    }
}
