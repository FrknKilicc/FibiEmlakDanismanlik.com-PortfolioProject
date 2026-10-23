using System.Collections.Generic;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    /// <summary>
    /// Benzer ilan bildirim formu ve detay sayfası kenar çubuğu widget'ı için görünüm modeli.
    /// </summary>
    public class ListingAlertFormVm
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ListingTypeId { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string> RoomCounts { get; set; } = new();
        public string? Note { get; set; }
        public int? SourceListingTypeId { get; set; }
        public int? SourceListingId { get; set; }
        public bool KvkkApproved { get; set; }

        /// <summary>
        /// İlan detay widget'ından gelen il adı (kimlik çözümlenmesi için).
        /// </summary>
        public string? CityName { get; set; }

        /// <summary>
        /// İlan detay widget'ından gelen ilçe adı (kimlik çözümlenmesi için).
        /// </summary>
        public string? DistrictName { get; set; }

        /// <summary>
        /// Konut ilanları için geçerli oda sayısı seçenekleri.
        /// </summary>
        public static readonly IReadOnlyList<string> RoomOptions = new[]
        {
            "1+0",
            "1+1",
            "2+1",
            "3+1",
            "4+1",
            "5+1 ve üzeri"
        };

        /// <summary>
        /// Desteklenen ilan türleri ve kullanıcı dostu başlıkları.
        /// 1: Satılık Arsa, 2: Satılık Konut, 3: Satılık İşyeri,
        /// 7: Kiralık Arsa, 8: Kiralık Konut, 9: Kiralık İşyeri.
        /// </summary>
        public static readonly IReadOnlyDictionary<int, string> ListingTypeOptions = new Dictionary<int, string>
        {
            { 1, "Satılık Arsa" },
            { 2, "Satılık Konut" },
            { 3, "Satılık İşyeri" },
            { 7, "Kiralık Arsa" },
            { 8, "Kiralık Konut" },
            { 9, "Kiralık İşyeri" }
        };

        /// <summary>
        /// Verilen ilan türünün konut olup olmadığını denetler (yalnızca 2 ve 8).
        /// </summary>
        public static bool IsHousing(int listingTypeId) => listingTypeId == 2 || listingTypeId == 8;
    }
}
