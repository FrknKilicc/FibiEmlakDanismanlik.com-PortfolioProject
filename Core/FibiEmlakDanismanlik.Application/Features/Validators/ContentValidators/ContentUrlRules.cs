using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    /// <summary>
    /// İçerik ve görsel bağlantılarına ilişkin doğrulama kurallarını sağlar.
    /// </summary>
    public static class ContentUrlRules
    {
        /// <summary>
        /// Görsel adresinin güvenli bir mutlak URL veya geçerli bir site içi göreli yol olup olmadığını doğrular.
        /// Protokolsüz harici adresleri (//) ve ters eğik çizgi (\) içeren yolları reddeder.
        /// </summary>
        /// <param name="url">Doğrulanacak görsel adresi veya yolu.</param>
        /// <returns>Görsel adresi geçerli ve güvenli ise true; aksi halde false.</returns>
        public static bool IsSafeImageUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var trimmed = url.Trim();

            if (trimmed.StartsWith("//", StringComparison.Ordinal) ||
                trimmed.StartsWith('\\') ||
                trimmed.Contains('\\'))
            {
                return false;
            }

            if (trimmed.Contains(':'))
            {
                return Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) &&
                       (uri.Scheme == "http" || uri.Scheme == "https");
            }

            return true;
        }
    }
}
