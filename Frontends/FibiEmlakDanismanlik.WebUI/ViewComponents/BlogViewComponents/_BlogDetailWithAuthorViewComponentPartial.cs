using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.BlogViewComponents
{
    public class _BlogDetailWithAuthorViewComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke(ResultBlogWithAuthorDto blog)
        {
            if (blog != null)
            {
                if (!string.IsNullOrWhiteSpace(blog.BlogTitle))
                {
                    HttpContext.Items[PageMeta.TitleItemKey] = blog.BlogTitle;
                }

                var desc = PageMeta.StripHtmlAndTruncate(blog.BlogDescription, PageMeta.DescriptionMaxLength);
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    HttpContext.Items[PageMeta.DescriptionItemKey] = desc;
                }
            }

            return View(blog);
        }
    }
}
