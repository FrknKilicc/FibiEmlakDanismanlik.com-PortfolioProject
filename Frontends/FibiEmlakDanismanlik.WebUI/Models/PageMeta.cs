using System.Net;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class PageMeta
    {
        public const string SiteName = "Fibi Yapı Emlak & Danışmanlık";
        public const string DefaultDescription = "Fibi Yapı Emlak & Danışmanlık: satılık ve kiralık konut, arsa ve işyeri ilanları, emlak danışmanlığı, bölge ve risk analizleri.";
        public const string TitleItemKey = "PageTitle";
        public const string DescriptionItemKey = "PageDescription";
        public const string CanonicalItemKey = "CanonicalPath";
        public const string ImageItemKey = "PageImage";
        public const string OgTypeItemKey = "PageOgType";
        public const string DefaultImagePath = "/images/page-title/page-title-bg.jpg";
        public const int DescriptionMaxLength = 155;

        /// <summary>
        /// HTML etiketlerini temizler, entity'leri çözer, ardışık boşlukları teke indirir
        /// ve belirtilen uzunluğu aşarsa son boşluktan keserek "…" ekler.
        /// </summary>
        public static string StripHtmlAndTruncate(string? html, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // HTML etiketlerini boşlukla değiştir
            var text = Regex.Replace(html, "<[^>]+>", " ");
            // HTML entity'leri çöz
            text = WebUtility.HtmlDecode(text);
            // Ardışık boşlukları teke indir
            text = Regex.Replace(text, @"\s+", " ");
            text = text.Trim();

            if (text.Length <= maxLength)
                return text;

            // Son boşluktan kes ve "…" ekle
            var truncated = text[..maxLength];
            var lastSpace = truncated.LastIndexOf(' ');
            if (lastSpace > 0)
                truncated = truncated[..lastSpace];

            return truncated + "…";
        }
    }

    public class PageMetaVm
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string Robots { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string OgType { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
    }
}
