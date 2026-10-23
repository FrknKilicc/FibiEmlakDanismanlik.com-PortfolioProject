using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces
{
    /// <summary>
    /// Yönetim paneli haber bekleyenler (ListingAlerts) arama ve filtreleme parametreleri.
    /// </summary>
    public class ListingAlertFilter
    {
        public ListingAlertStatus? Status { get; set; }
        public int? AgentId { get; set; }
        public bool? Unassigned { get; set; }
        public bool OnlyWithPending { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }

    /// <summary>
    /// Oturum açmış kullanıcının rolüne göre haber bekleyenler erişim kapsamı.
    /// </summary>
    public class ListingAlertScope
    {
        public bool SeeAll { get; set; }
        public int? AgentId { get; set; }
    }

    /// <summary>
    /// Yönetim paneli haber bekleyen talep listesi satırı.
    /// </summary>
    public class ListingAlertRow
    {
        public int ListingAlertId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ListingTypeId { get; set; }
        public string TypeDisplay { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? RoomCounts { get; set; }
        public string? NotePreview { get; set; }
        public ListingAlertStatus Status { get; set; }
        public int? AssignedAgentId { get; set; }
        public string? AssignedAgentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PendingMatchCount { get; set; }
        public int NotifiedCount { get; set; }
        public int? SourceListingTypeId { get; set; }
        public int? SourceListingId { get; set; }
        public string? SourceListingTitle { get; set; }
    }

    /// <summary>
    /// Haber bekleyen talep detay görünümü, bekleyen eşleşmeler ve bildirim geçmişi.
    /// </summary>
    public class ListingAlertDetail : ListingAlertRow
    {
        public string? Note { get; set; }
        public DateTime KvkkApprovedAt { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
        public List<ListingAlertListingItem> PendingMatches { get; set; } = new List<ListingAlertListingItem>();
        public List<ListingAlertMatchItem> History { get; set; } = new List<ListingAlertMatchItem>();
    }

    /// <summary>
    /// Talep ile eşleşen veya bildirilen ilan bilgisi.
    /// </summary>
    public class ListingAlertListingItem
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? NumberOfRoom { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AgentId { get; set; }
        public string? AgentName { get; set; }
    }

    /// <summary>
    /// Talep bildirim/eşleşme hareket geçmişi satırı.
    /// </summary>
    public class ListingAlertMatchItem
    {
        public int ListingAlertMatchId { get; set; }
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public string? ListingTitle { get; set; }
        public ListingAlertMatchStatus Status { get; set; }
        public string? AgentName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }

    /// <summary>
    /// Yönetim paneli haber bekleyenler durum ve sayaç özeti.
    /// </summary>
    public class ListingAlertSummary
    {
        public int Active { get; set; }
        public int ActiveWithPending { get; set; }
        public int Closed { get; set; }
        public int Unassigned { get; set; }
    }

    /// <summary>
    /// Belirli bir ilanın kriterlerine uyan aktif haber talebi satırı.
    /// </summary>
    public class ListingAlertForListing
    {
        public int ListingAlertId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AssignedAgentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public ListingAlertMatchStatus? MatchStatus { get; set; }
    }
}
