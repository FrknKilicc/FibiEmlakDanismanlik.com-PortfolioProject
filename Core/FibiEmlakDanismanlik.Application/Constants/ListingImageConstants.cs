using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// İlan görselleri, yükleme sınırları, boyutlandırma ve tablo eşlemeleri için sabit tanımlar.
    /// </summary>
    public static class ListingImageConstants
    {
        /// <summary>
        /// Bir ilana eklenebilecek maksimum görsel sayısı (eski PropImgUrl1..30 kolon sayısı ile uyumlu).
        /// </summary>
        public const int MaxImagesPerListing = 30;

        /// <summary>
        /// Tek bir görsel için izin verilen maksimum yükleme boyutu (bayt cinsinden, 15 MB).
        /// </summary>
        public const int MaxUploadBytes = 15 * 1024 * 1024;

        /// <summary>
        /// Tek bir istekte yüklenebilecek maksimum dosya sayısı.
        /// </summary>
        public const int MaxUploadFilesPerRequest = 20;

        /// <summary>
        /// Yüklenen görsel için izin verilen maksimum piksel boyutu (genişlik veya yükseklik).
        /// </summary>
        public const int MaxPixelDimension = 12000;

        /// <summary>
        /// Büyük boy görsel genişliği (piksel).
        /// </summary>
        public const int LargeWidth = 1600;

        /// <summary>
        /// Orta boy görsel genişliği (piksel).
        /// </summary>
        public const int MediumWidth = 800;

        /// <summary>
        /// Küçük resim (thumbnail) görsel genişliği (piksel).
        /// </summary>
        public const int ThumbWidth = 400;

        /// <summary>
        /// WebP sıkıştırma kalite oranı (0-100).
        /// </summary>
        public const int WebpQuality = 80;

        /// <summary>
        /// Yüklenen dosyaların sunulduğu göreli istek yolu öneki.
        /// </summary>
        public const string UploadsRequestPath = "/uploads";

        /// <summary>
        /// İlan görsellerinin saklandığı klasör adı.
        /// </summary>
        public const string ListingsFolder = "listings";

        /// <summary>
        /// İlan türü kimliğine göre veritabanı tablo adı ve birincil anahtar kolon adı eşlemeleri.
        /// </summary>
        public static readonly IReadOnlyDictionary<int, (string Table, string KeyColumn)> ListingTables =
            new Dictionary<int, (string Table, string KeyColumn)>
            {
                [1] = ("forSaleLandListings", "ForSaleLandListingId"),
                [2] = ("forSaleHousingPropertyListings", "ForSaleHousingListId"),
                [3] = ("forSaleCommercialPropertyListings", "ForSaleCommercialListingId"),
                [7] = ("rentalLandListings", "RentalLandListingId"),
                [8] = ("rentalHousingListings", "RentalHousingListId"),
                [9] = ("rentalCommercialPropertyListings", "RentalCommercialListId")
            };

        /// <summary>
        /// Verilen ilan türü kimliğinin geçerli ve bilinen bir ilan tablosuna karşılık gelip gelmediğini doğrular.
        /// </summary>
        /// <param name="listingTypeId">Doğrulanacak ilan türü kimliği.</param>
        /// <returns>İlan türü biliniyorsa true, aksi halde false.</returns>
        public static bool IsKnownListingType(int listingTypeId)
        {
            return ListingTables.ContainsKey(listingTypeId);
        }
    }
}
