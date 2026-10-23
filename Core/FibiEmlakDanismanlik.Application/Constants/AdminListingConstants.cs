using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// Yönetim paneli ilan işlemleri için sabit tanımlar ve yardımcı metotlar.
    /// </summary>
    public static class AdminListingConstants
    {
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;

        /// <summary>
        /// Desteklenen tüm ilan türü kimlikleri (1: Satılık Arsa, 2: Satılık Konut, 3: Satılık İşyeri, 7: Kiralık Arsa, 8: Kiralık Konut, 9: Kiralık İşyeri).
        /// </summary>
        public static readonly IReadOnlyList<int> AllListingTypeIds = new[] { 1, 2, 3, 7, 8, 9 };

        /// <summary>
        /// İlanların alabileceği tüm durumlar.
        /// </summary>
        public static readonly IReadOnlyList<string> AllStatuses = new[]
        {
            PropertyStatuses.Aktif,
            PropertyStatuses.Opsiyonlu,
            PropertyStatuses.Pasif,
            PropertyStatuses.Satildi,
            PropertyStatuses.Kiralandi
        };

        /// <summary>
        /// İlan türü kimliğinin kiralık olup olmadığını belirler (7: Arsa, 8: Konut, 9: İşyeri).
        /// </summary>
        public static bool IsRent(int listingTypeId) => listingTypeId is 7 or 8 or 9;

        /// <summary>
        /// İlan türü kimliğine göre kategori kodunu döner ("housing", "land", "commercial").
        /// </summary>
        public static string Category(int listingTypeId) => listingTypeId switch
        {
            1 or 7 => "land",
            2 or 8 => "housing",
            3 or 9 => "commercial",
            _ => string.Empty
        };

        /// <summary>
        /// Kategori koduna veya ilan türü kimliğine göre kullanıcı dostu kategori adını döner.
        /// </summary>
        public static string CategoryDisplay(string category) => (category?.ToLowerInvariant()) switch
        {
            "housing" => "Konut",
            "land" => "Arsa",
            "commercial" => "İşyeri",
            _ => category ?? string.Empty
        };

        /// <summary>
        /// İlan türü kimliğine göre kullanıcı dostu kategori adını döner.
        /// </summary>
        public static string CategoryDisplay(int listingTypeId) => CategoryDisplay(Category(listingTypeId));

        /// <summary>
        /// Kiralık/satılık durumu ve kategori koduna göre ilan türü kimliğini döner. Bulunamazsa 0 döner.
        /// </summary>
        public static int ListingTypeIdFor(bool isRent, string category)
        {
            var normalized = category?.Trim().ToLowerInvariant();
            return (isRent, normalized) switch
            {
                (false, "land") => 1,
                (false, "housing") => 2,
                (false, "commercial") => 3,
                (true, "land") => 7,
                (true, "housing") => 8,
                (true, "commercial") => 9,
                _ => 0
            };
        }
    }
}
