using AngleSharp.Dom;
using Ganss.Xss;
using Microsoft.AspNetCore.Html;
using System;

namespace FibiEmlakDanismanlik.WebUI.Services
{
    /// <summary>
    /// Kullanıcı/admin kaynaklı HTML içeriklerini izinli etiket ve öznitelik listesine göre temizleyen servis.
    /// Thread-safe singleton olarak kullanılır.
    /// </summary>
    public class HtmlContentSanitizer
    {
        private readonly HtmlSanitizer _sanitizer;
        private readonly object _syncLock = new();

        public HtmlContentSanitizer()
        {
            _sanitizer = new HtmlSanitizer();

            // İzinli etiketler yalnızca metin biçimlendirme için
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("i");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("a");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("h4");
            _sanitizer.AllowedTags.Add("h5");
            _sanitizer.AllowedTags.Add("h6");
            _sanitizer.AllowedTags.Add("blockquote");
            _sanitizer.AllowedTags.Add("span");

            // İzinli öznitelikler: a için href, title, target, rel
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.Add("href");
            _sanitizer.AllowedAttributes.Add("title");
            _sanitizer.AllowedAttributes.Add("target");
            _sanitizer.AllowedAttributes.Add("rel");

            // İzinli şemalar: http, https, mailto, tel
            _sanitizer.AllowedSchemes.Clear();
            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("https");
            _sanitizer.AllowedSchemes.Add("mailto");
            _sanitizer.AllowedSchemes.Add("tel");

            // CSS style ve tüm diğer stil kurallarına izin verme
            _sanitizer.AllowedCssProperties.Clear();
            _sanitizer.AllowedAtRules.Clear();

            // target özniteliği olan linklere rel="noopener noreferrer" ekle ve a dışındaki etiketlerden a'ya özel öznitelikleri kaldır
            _sanitizer.PostProcessNode += (sender, args) =>
            {
                if (args.Node is IElement element)
                {
                    if (string.Equals(element.TagName, "A", StringComparison.OrdinalIgnoreCase))
                    {
                        if (element.HasAttribute("target"))
                        {
                            element.SetAttribute("rel", "noopener noreferrer");
                        }
                    }
                    else
                    {
                        element.RemoveAttribute("href");
                        element.RemoveAttribute("title");
                        element.RemoveAttribute("target");
                        element.RemoveAttribute("rel");
                    }
                }
            };
        }

        /// <summary>
        /// Verilen HTML içeriği izinli liste kurallarına göre temizler. Boş veya null için HtmlString.Empty döner.
        /// </summary>
        public IHtmlContent Sanitize(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return HtmlString.Empty;
            }

            string sanitized;
            lock (_syncLock)
            {
                sanitized = _sanitizer.Sanitize(html);
            }

            return new HtmlString(sanitized);
        }
    }
}
