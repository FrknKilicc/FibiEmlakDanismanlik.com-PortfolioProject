namespace FibiEmlakDanismanlik.WebUI.Models.ListingsMap
{
    /// <summary>
    /// Haritada İlanlar sayfası görünüm modeli.
    /// </summary>
    public class ListingsMapPageVm
    {
        /// <summary>
        /// API taban adresi (sonu /api/ ile biter).
        /// </summary>
        public string ApiUrl { get; set; } = string.Empty;

        /// <summary>
        /// Başlangıç kullanım filtresi ("" | "sale" | "rent").
        /// </summary>
        public string InitialUsage { get; set; } = string.Empty;

        /// <summary>
        /// Başlangıç kategori filtresi ("" | "housing" | "land" | "commercial").
        /// </summary>
        public string InitialCategory { get; set; } = string.Empty;
    }
}
