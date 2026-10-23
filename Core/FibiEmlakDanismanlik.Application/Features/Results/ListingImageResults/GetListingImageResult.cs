namespace FibiEmlakDanismanlik.Application.Features.Results.ListingImageResults
{
    /// <summary>
    /// İlan görseline ait temel bilgileri ve farklı boyut adreslerini döndüren sonuç sınıfı.
    /// </summary>
    public class GetListingImageResult
    {
        /// <summary>
        /// Görsel birincil anahtar kimliği.
        /// </summary>
        public int ListingImageId { get; set; }

        /// <summary>
        /// Büyük boy görsel adresi (veya tekil görsel adresi).
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Orta boy görsel adresi.
        /// </summary>
        public string? MediumUrl { get; set; }

        /// <summary>
        /// Küçük resim (thumbnail) görsel adresi.
        /// </summary>
        public string? ThumbUrl { get; set; }

        /// <summary>
        /// Görsel genişliği (piksel).
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Görsel yüksekliği (piksel).
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Erişilebilirlik ve SEO için alternatif metin.
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Kapak / vitrin görseli olup olmadığını belirtir.
        /// </summary>
        public bool IsCover { get; set; }

        /// <summary>
        /// Görselin sıralama düzeni.
        /// </summary>
        public int SortOrder { get; set; }
    }
}
