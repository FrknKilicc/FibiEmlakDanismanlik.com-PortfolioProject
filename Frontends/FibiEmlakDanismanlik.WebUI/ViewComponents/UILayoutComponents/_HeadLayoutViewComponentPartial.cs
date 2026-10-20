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

            var canonicalPath = HttpContext.Items[PageMeta.CanonicalItemKey] as string;
            if (string.IsNullOrWhiteSpace(canonicalPath))
            {
                var path = HttpContext.Request.Path.Value;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = "/";
                }

                if (HttpContext.Request.Query.TryGetValue("page", out var pageValues) &&
                    int.TryParse(pageValues.FirstOrDefault(), out var pageNum) &&
                    pageNum > 1)
                {
                    canonicalPath = $"{path}?page={pageNum}";
                }
                else
                {
                    canonicalPath = path;
                }
            }

            var canonicalUrl = SeoUrl.Absolute(HttpContext, canonicalPath);

            return View(new PageMetaVm
            {
                Title = title,
                Description = description,
                CanonicalUrl = canonicalUrl
            });
        }
    }
}
