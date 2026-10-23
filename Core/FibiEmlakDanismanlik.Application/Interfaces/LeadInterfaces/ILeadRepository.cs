using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces
{
    /// <summary>
    /// Yönetim paneli müşteri talepleri (lead) okuma, filtreleme, durum yönetimi, danışman atama ve hareket geçmişi repository arayüzü.
    /// </summary>
    public interface ILeadRepository
    {
        /// <summary>
        /// Filtreleme kriterlerine ve kullanıcı yetki kapsamına göre birleşik müşteri taleplerini arar, sıralar ve sayfalar.
        /// </summary>
        Task<(int Total, List<LeadRow> Rows)> SearchAsync(LeadFilter f, LeadScope s);

        /// <summary>
        /// Kullanıcının görebileceği talep kapsamı üzerinden toplam, durumlara göre dağılım, atanmamış ve yaklaşan sayaçları döner.
        /// </summary>
        Task<LeadSummary> GetSummaryAsync(LeadScope s);

        /// <summary>
        /// Belirtilen kaynak ve kimliğe sahip talebin ayrıntılı bilgilerini, form verisini ve hareket geçmişini döner.
        /// </summary>
        Task<LeadDetail?> GetDetailAsync(LeadSource source, int leadId);

        /// <summary>
        /// Talebin atanmış danışmanını ve (varsa) bağlı ilanın danışman kimliğini döner. Talep bulunamazsa null döner.
        /// </summary>
        Task<(int? AssignedAgentId, int? ListingAgentId)?> GetOwnershipAsync(LeadSource source, int leadId);

        /// <summary>
        /// Talebin takip durumunu günceller ve son güncelleme zamanını kaydeder (FİBİ talebinde New dışı ise IsContacted=true yapılır).
        /// </summary>
        Task<bool> UpdateStatusAsync(LeadSource source, int leadId, LeadStatus status);

        /// <summary>
        /// Talebi bir danışmana atar veya atamasını kaldırır.
        /// </summary>
        Task<bool> AssignAsync(LeadSource source, int leadId, int? agentId);

        /// <summary>
        /// Talebe ait yeni bir hareket, not veya durum değişikliği kaydı ekler.
        /// </summary>
        Task AddActivityAsync(LeadActivity a);

        /// <summary>
        /// Belirli bir ilana ait iletişim ve randevu taleplerini sıralı liste olarak döner.
        /// </summary>
        Task<List<LeadRow>> GetByListingAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Belirtilen danışmanın mevcut ve aktif olup olmadığını doğrular.
        /// </summary>
        Task<bool> IsActiveAgentAsync(int agentId);
    }
}
