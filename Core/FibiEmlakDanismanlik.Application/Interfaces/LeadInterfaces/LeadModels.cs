using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces
{
    /// <summary>
    /// Yönetim paneli müşteri talepleri arama ve filtreleme parametreleri.
    /// </summary>
    public class LeadFilter
    {
        public LeadSource? Source { get; set; }
        public LeadStatus? Status { get; set; }
        public int? AgentId { get; set; }
        public bool? Unassigned { get; set; }
        public int? ListingTypeId { get; set; }
        public int? ListingId { get; set; }
        public string? Search { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }

    /// <summary>
    /// Oturum açmış kullanıcının rolüne göre talep erişim kapsamı.
    /// </summary>
    public class LeadScope
    {
        public bool SeeAll { get; set; }
        public int? AgentId { get; set; }
    }

    /// <summary>
    /// Yönetim paneli müşteri talepleri liste satırı.
    /// </summary>
    public class LeadRow
    {
        public LeadSource Source { get; set; }
        public int LeadId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? MessagePreview { get; set; }
        public LeadStatus Status { get; set; }
        public int? AssignedAgentId { get; set; }
        public string? AssignedAgentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public int? ListingTypeId { get; set; }
        public int? ListingId { get; set; }
        public string? ListingTitle { get; set; }
        public string? ServiceModule { get; set; }
        public string? NeighborhoodName { get; set; }
        public DateTime? LastActivityAtUtc { get; set; }
    }

    /// <summary>
    /// Talep hareket, not veya durum değişikliği geçmişi satırı.
    /// </summary>
    public class LeadActivityItem
    {
        public int ActivityId { get; set; }
        public LeadActivityType Type { get; set; }
        public string? Text { get; set; }
        public LeadStatus? OldStatus { get; set; }
        public LeadStatus? NewStatus { get; set; }
        public int? AgentId { get; set; }
        public string? AgentName { get; set; }
        public string? AssignedAgentName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    /// <summary>
    /// Müşteri talebinin ayrıntılı görünümü, tam metin ve hareket geçmişi.
    /// </summary>
    public class LeadDetail : LeadRow
    {
        public string? Message { get; set; }
        public string? CustomerNote { get; set; }
        public string? CalculationSummaryJson { get; set; }

        /// <summary>
        /// KVKK aydınlatma metni onay zamanı (UTC).
        /// </summary>
        public DateTime? KvkkApprovedAt { get; set; }
        public List<LeadActivityItem> Activities { get; set; } = new List<LeadActivityItem>();
    }

    /// <summary>
    /// Yönetim paneli talep sayaçları ve durum özeti.
    /// </summary>
    public class LeadSummary
    {
        public int Total { get; set; }
        public Dictionary<string, int> ByStatus { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public int Unassigned { get; set; }
        public int NewLast7Days { get; set; }
        public int AppointmentsUpcoming { get; set; }
    }
}
