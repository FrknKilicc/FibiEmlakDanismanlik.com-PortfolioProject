using System;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// İlan fotoğraflarını ve farklı boyutlardaki görsel adreslerini temsil eden varlık sınıfı.
    /// </summary>
    public class ListingImage
    {
        /// <summary>
        /// Görsel birincil anahtarı.
        /// </summary>
        [Key]
        public int ListingImageId { get; set; }

        /// <summary>
        /// İlan türü kimliği (1: Satılık Arsa, 2: Satılık Konut, 3: Satılık İşyeri, 7: Kiralık Arsa, 8: Kiralık Konut, 9: Kiralık İşyeri).
        /// </summary>
        public int ListingTypeId { get; set; }

        /// <summary>
        /// İlanın ilgili tablodaki birincil anahtar kimliği.
        /// </summary>
        public int ListingId { get; set; }

        /// <summary>
        /// En büyük boy görsel adresi (eski kayıtlarda mevcut adres).
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
        /// Görselin sıralama düzeni (1'den başlar).
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Görselin kapak / vitrin görseli olup olmadığını belirtir.
        /// </summary>
        public bool IsCover { get; set; }

        /// <summary>
        /// Görselin türü (Fotoğraf, Kat Planı, Kroki, İmar Planı).
        /// </summary>
        public ListingImageKind Kind { get; set; } = ListingImageKind.Photo;

        /// <summary>
        /// Plan veya görsel başlığı (ör. "Zemin kat").
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Görsel için alternatif metin (erişilebilirlik ve SEO amaçlı).
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Orijinal / büyük görselin piksel cinsinden genişliği.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Orijinal / büyük görselin piksel cinsinden yüksekliği.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Yüklenen dosyaların sunucudaki göreli klasör ve ad öneki (eski kayıtlarda null).
        /// </summary>
        public string? StoragePath { get; set; }

        /// <summary>
        /// Görsel kaydının oluşturulduğu UTC tarih ve saat.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
