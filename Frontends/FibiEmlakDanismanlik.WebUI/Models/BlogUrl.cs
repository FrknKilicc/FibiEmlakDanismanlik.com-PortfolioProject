namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class BlogUrl
    {
        /// <summary>
        /// Blog yazısı için SEO dostu detay adresi üretir (/blog/{slug}-{blogId} veya slug boşsa /blog/{blogId}).
        /// </summary>
        public static string DetailPath(int blogId, string? title)
        {
            var slug = ListingUrl.Slugify(title);
            return string.IsNullOrEmpty(slug) ? $"/blog/{blogId}" : $"/blog/{slug}-{blogId}";
        }
    }
}
