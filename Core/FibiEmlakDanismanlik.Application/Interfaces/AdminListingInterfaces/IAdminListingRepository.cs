using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces
{
    /// <summary>
    /// Yönetim paneli ilan okuma, filtreleme, özet, yardımcı veri erişim ve yazma arayüzü.
    /// </summary>
    public interface IAdminListingRepository
    {
        /// <summary>
        /// İlanın ait olduğu danışman kimliğini döner. İlan yoksa null döner.
        /// </summary>
        Task<int?> GetListingAgentIdAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Danışmanın tüm ilanları görme yetkisi (CanViewAllListings) olup olmadığını sorgular.
        /// </summary>
        Task<bool> GetCanViewAllListingsAsync(int agentId);

        /// <summary>
        /// Danışmana atanmış özel ilan erişim izinlerini döner.
        /// </summary>
        Task<List<ListingAccessGrant>> GetGrantsForAgentAsync(int agentId);

        /// <summary>
        /// Verilen filtre ve kapsam kurallarına göre birleşik ilanları arar, sıralar ve sayfalar.
        /// </summary>
        Task<(int Total, List<AdminListingRow> Rows)> SearchAsync(AdminListingFilter filter, ListingScope scope);

        /// <summary>
        /// Kullanıcının görebileceği ilan kapsamı üzerinden durum ve kategori sayaçlarını döner.
        /// </summary>
        Task<AdminListingSummary> GetSummaryAsync(ListingScope scope);

        /// <summary>
        /// İlan türü ve kimliğine göre birleştirilmiş ilan detayını, olanaklarını ve konumunu döner.
        /// </summary>
        Task<AdminListingDetail?> GetDetailAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Sistemde tanımlı aktif olanakları sıralı olarak döner.
        /// </summary>
        Task<List<AmenityDefinition>> GetAmenitiesAsync();

        /// <summary>
        /// Konut kategorilerini döner.
        /// </summary>
        Task<List<HousingCategory>> GetHousingCategoriesAsync();

        /// <summary>
        /// Arsa kategorilerini döner.
        /// </summary>
        Task<List<LandCategory>> GetLandCategoriesAsync();

        /// <summary>
        /// Aktif danışmanların kimlik ve ad listesini ada göre sıralı döner.
        /// </summary>
        Task<List<(int AgentId, string AgentName)>> GetActiveAgentsAsync();

        /// <summary>
        /// İlan kaydeder (ekler veya günceller) ve olanak ara tablosunu günceller.
        /// </summary>
        Task<(int ListingTypeId, int ListingId)> SaveAsync(SaveAdminListingCommand cmd, int agentId, bool isNew);

        /// <summary>
        /// İlanın durumunu günceller; Aktif/Opsiyonlu dışına çıkarsa BestDeals = false yapar.
        /// </summary>
        Task<bool> SetStatusAsync(int listingTypeId, int listingId, string status);

        /// <summary>
        /// İlanın mevcut durumunu döner. Bulunamazsa null döner.
        /// </summary>
        Task<string?> GetListingStatusAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Satılık veya kiralık vitrinde (BestDeals=true) bulunan aktif ilan sayısını döner.
        /// </summary>
        Task<int> CountBestDealsAsync(bool isRent);

        /// <summary>
        /// İlanın vitrin (BestDeals) durumunu günceller.
        /// </summary>
        Task<bool> SetBestDealAsync(int listingTypeId, int listingId, bool isBestDeal);

        /// <summary>
        /// İlanın harita koordinatını manuel kaynakla ve building hassasiyetiyle kaydeder.
        /// </summary>
        Task<bool> SetGeoPointAsync(int listingTypeId, int listingId, decimal latitude, decimal longitude);

        /// <summary>
        /// İlanı ve bağlı tüm kayıtları (olanaklar, harita pini, izinler, yakındaki yerler, görseller) siler ve silinen görsellerin dosya yollarını döner.
        /// </summary>
        Task<List<string>> DeleteListingAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Belirli bir ilana tanımlanmış özel erişim izinlerini döner.
        /// </summary>
        Task<List<ListingAccessGrant>> GetGrantsForListingAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Bir ilana danışman için özel erişim izni ekler veya mevcutsa günceller.
        /// </summary>
        Task AddOrUpdateGrantAsync(int listingTypeId, int listingId, int targetAgentId, ListingAccessLevel access, int grantedByAgentId);

        /// <summary>
        /// Belirtilen erişim iznini siler.
        /// </summary>
        Task<bool> RemoveGrantAsync(int listingAccessGrantId);

        /// <summary>
        /// Belirtilen danışmanın var ve aktif olup olmadığını sorgular.
        /// </summary>
        Task<bool> AgentExistsAndActiveAsync(int agentId);

        /// <summary>
        /// Belirtilen danışmanın var ve aktif olup olmadığını sorgular.
        /// </summary>
        Task<bool> IsActiveAgentAsync(int agentId);

        /// <summary>
        /// Konut kategorisinin mevcut olup olmadığını doğrular.
        /// </summary>
        Task<bool> HousingCategoryExistsAsync(int id);

        /// <summary>
        /// Arsa kategorisinin mevcut olup olmadığını doğrular.
        /// </summary>
        Task<bool> LandCategoryExistsAsync(int id);

        /// <summary>
        /// Danışman bilgilerini döner.
        /// </summary>
        Task<Agent?> GetAgentByIdAsync(int agentId);

        /// <summary>
        /// Aktif yakın yer kategorilerini ve ilanın tüm yakın yer kayıtlarını (kategori bilgisiyle) döner.
        /// </summary>
        Task<(List<NearbyCategory> Categories, List<ListingNearbyPlace> Places)> GetNearbyPlacesAsync(int listingTypeId, int listingId);

        /// <summary>
        /// İlanın tüm yakın yer kayıtlarını silip verilen listeyi (sırasıyla) yazar.
        /// </summary>
        Task<bool> ReplaceNearbyPlacesAsync(int listingTypeId, int listingId, List<(int NearbyCategoryId, string PlaceName, decimal DistanceKm)> places);
    }
}
