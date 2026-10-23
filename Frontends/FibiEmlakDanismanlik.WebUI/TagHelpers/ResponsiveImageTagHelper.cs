using System;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FibiEmlakDanismanlik.WebUI.TagHelpers
{
    /// <summary>
    /// İlan fotoğrafları için duyarlı (responsive) görsel etiketleri (srcset ve sizes) üreten Tag Helper.
    /// Sunucuda üç boyutta (-400, -800, -1600) saklanan WebP görsellerini otomatik olarak duyarlı hale getirir.
    /// </summary>
    [HtmlTargetElement("img", Attributes = "fibi-responsive", TagStructure = TagStructure.WithoutEndTag)]
    public class ResponsiveImageTagHelper : TagHelper
    {
        private static readonly Regex ImageRegex = new(
            @"^(?<base>.+)-(1600|800|400)\.webp(?<q>\?.*)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Duyarlı görsel için sizes özniteliği değeri. Boş bırakılırsa varsayılan olarak "100vw" kullanılır.
        /// </summary>
        [HtmlAttributeName("fibi-sizes")]
        public string? Sizes { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(output);

            // fibi-responsive özniteliği her durumda çıktıdan kaldırılır (fibi-sizes zaten model bağlama ile tüketilir)
            output.Attributes.RemoveAll("fibi-responsive");

            var rawSrc = output.Attributes["src"]?.Value;
            var src = ExtractSrc(rawSrc);

            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            var match = ImageRegex.Match(src);
            if (!match.Success)
            {
                return;
            }

            // Zaten bir srcset özniteliği varsa dokunma
            if (output.Attributes.ContainsName("srcset"))
            {
                return;
            }

            var baseUrl = match.Groups["base"].Value;
            var q = match.Groups["q"].Value;

            var srcset = $"{baseUrl}-400.webp{q} 400w, {baseUrl}-800.webp{q} 800w, {baseUrl}-1600.webp{q} 1600w";
            var sizes = !string.IsNullOrWhiteSpace(Sizes) ? Sizes : "100vw";

            output.Attributes.SetAttribute("srcset", srcset);
            output.Attributes.SetAttribute("sizes", sizes);
            output.Attributes.SetAttribute("src", $"{baseUrl}-800.webp{q}");
        }

        /// <summary>
        /// Kalıba uyan görsel adresinin boyutunu değiştirir. Kalıba uymayan adresleri aynen döndürür.
        /// </summary>
        /// <param name="url">Dönüştürülecek görsel adresi.</param>
        /// <param name="width">İstenen genişlik (ör. 400, 800, 1600).</param>
        /// <returns>Boyutlandırılmış görsel adresi veya orijinal adres.</returns>
        public static string? SizedUrl(string? url, int width)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            var match = ImageRegex.Match(url);
            if (!match.Success)
            {
                return url;
            }

            var baseUrl = match.Groups["base"].Value;
            var q = match.Groups["q"].Value;

            return $"{baseUrl}-{width}.webp{q}";
        }

        private static string? ExtractSrc(object? rawValue)
        {
            if (rawValue == null)
            {
                return null;
            }

            if (rawValue is string str)
            {
                return str;
            }

            if (rawValue is HtmlString htmlString)
            {
                return htmlString.Value;
            }

            if (rawValue is IHtmlContent htmlContent)
            {
                using var writer = new StringWriter();
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                return WebUtility.HtmlDecode(writer.ToString());
            }

            return rawValue.ToString();
        }
    }
}
