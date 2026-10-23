using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces
{
    /// <summary>
    /// Benzer ilan bildirim talepleri (haber bekleyenler) veri erişim arayüzü.
    /// </summary>
    public interface IListingAlertRepository
    {
        /// <summary>
        /// Belirtilen ilin sistemde kayıtlı olup olmadığını doğrular.
        /// </summary>
        Task<bool> CityExistsAsync(int cityId);

        /// <summary>
        /// Belirtilen ilçenin verilen ile ait olup olmadığını doğrular.
        /// </summary>
        Task<bool> DistrictBelongsToCityAsync(int districtId, int cityId);

        /// <summary>
        /// Verilen telefon numarasına ait aktif durumdaki bildirim talebi sayısını döner.
        /// </summary>
        Task<int> CountActiveByPhoneAsync(string phone);

        /// <summary>
        /// Yeni bir haber talebi kaydeder.
        /// </summary>
        Task AddAsync(ListingAlert alert);

        /// <summary>
        /// Yönetim paneli için haber taleplerini filtreler, yetki kapsamına göre listeler ve sayfalar.
        /// </summary>
        Task<(int Total, List<ListingAlertRow> Rows)> SearchAsync(ListingAlertFilter f, ListingAlertScope s);

        /// <summary>
        /// Kullanıcı yetki kapsamındaki aktif, bekleyen eşleşmesi olan, kapalı ve atamasız talep sayaçlarını döner.
        /// </summary>
        Task<ListingAlertSummary> GetSummaryAsync(ListingAlertScope s);

        /// <summary>
        /// Belirli bir haber talebinin ayrıntılı bilgilerini, bekleyen eşleşmelerini ve işlem geçmişini döner.
        /// </summary>
        Task<ListingAlertDetail?> GetDetailAsync(int id);

        /// <summary>
        /// Talebe atanan danışman kimliğini döner.
        /// </summary>
        Task<int?> GetAssignedAgentIdAsync(int id);

        /// <summary>
        /// Belirtilen kimliğe sahip bir haber talebinin var olup olmadığını doğrular.
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Belirtilen aktif haber talebi için bekleyen eşleşen ilanların listesini döner.
        /// </summary>
        Task<List<ListingAlertListingItem>> GetPendingMatchesAsync(int id);

        /// <summary>
        /// Haber talebinin takip durumunu günceller (Kapandığında ClosedAtUtc atanır, Aktif yapıldığında sıfırlanır).
        /// </summary>
        Task<bool> SetStatusAsync(int id, ListingAlertStatus status);

        /// <summary>
        /// Talebi bir danışmana atar veya danışman atamasını kaldırır.
        /// </summary>
        Task<bool> AssignAsync(int id, int? agentId);

        /// <summary>
        /// Danışmanın sistemde aktif durumda olup olmadığını doğrular.
        /// </summary>
        Task<bool> IsActiveAgentAsync(int agentId);

        /// <summary>
        /// Talep-ilan eşleşme veya bildirim durumunu ekler veya mevcutsa günceller.
        /// </summary>
        Task UpsertMatchAsync(int alertId, int listingTypeId, int listingId, ListingAlertMatchStatus status, int? agentId);

        /// <summary>
        /// Belirli bir ilanın kriterlerine uyan aktif haber taleplerini kullanıcı kapsamına göre döner.
        /// </summary>
        Task<List<ListingAlertForListing>> GetAlertsForListingAsync(int listingTypeId, int listingId, ListingAlertScope s);

        /// <summary>
        /// Belirli bir ilanın kriterlerine uyan ancak kullanıcının yetki kapsamı dışında kalan aktif haber talebi sayısını döner.
        /// </summary>
        Task<int> CountAlertsForListingOutsideScopeAsync(int listingTypeId, int listingId, ListingAlertScope s);
    }
}
