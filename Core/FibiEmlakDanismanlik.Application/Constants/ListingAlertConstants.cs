using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// Benzer ilan bildirimi (haber bekleyenler) sabitleri.
    /// WebUI tüm ziyaretçiler adına tek IP'den geldiği için bu sınır yalnızca kaba bir üst sınırdır;
    /// asıl ziyaretçi bazlı sınır WebUI'deki FormSpamGuard'da uygulanır.
    /// </summary>
    public static class ListingAlertConstants
    {
        public const string RateLimitPolicy = "listing-alert";
        public const int RateLimitPermitCount = 30;
        public const int RateLimitWindowMinutes = 1;

        /// <summary>
        /// Konut kategorisine ait ilan türü kimlikleri (2: Satılık Konut, 8: Kiralık Konut).
        /// </summary>
        public static readonly int[] HousingTypeIds = { 2, 8 };

        /// <summary>
        /// Sistemde geçerli tüm ilan türü kimlikleri (1: Satılık Arsa, 2: Satılık Konut, 3: Satılık İşyeri, 7: Kiralık Arsa, 8: Kiralık Konut, 9: Kiralık İşyeri).
        /// </summary>
        public static readonly int[] ValidListingTypeIds = { 1, 2, 3, 7, 8, 9 };

        /// <summary>
        /// Konut aramalarında seçilebilecek oda sayısı seçenekleri (WebUI ve validator ortak).
        /// </summary>
        public static readonly string[] RoomOptions =
        {
            "1+0",
            "1+1",
            "2+1",
            "3+1",
            "4+1",
            "5+1 ve üzeri"
        };

        /// <summary>
        /// Tek bir telefon numarası için izin verilen azami aktif haber talebi sayısı.
        /// </summary>
        public const int MaxActivePerPhone = 5;
    }
}
