namespace FibiEmlakDanismanlik.WebUI.Models
{
    /// <summary>
    /// Tarayıcıdaki betiklerin (harita, filtreler, arama önerileri, FİBİ Hizmetler) çağıracağı API adresi.
    /// Sunucu tarafı istekler Url:ApiUrl ile iç adrese gider; canlıda API dışarıya yalnızca ters vekil üzerinden
    /// (ör. https://fibiemlak.com/api/) açıldığı için tarayıcıya Url:PublicApiUrl verilir. Boşsa Url:ApiUrl kullanılır.
    /// </summary>
    public static class BrowserApiUrl
    {
        public static string? GetBrowserApiUrl(this IConfiguration configuration)
        {
            var publicApiUrl = configuration["Url:PublicApiUrl"];
            return string.IsNullOrWhiteSpace(publicApiUrl) ? configuration["Url:ApiUrl"] : publicApiUrl;
        }
    }
}
