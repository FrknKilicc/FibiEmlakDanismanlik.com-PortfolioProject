using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Interfaces.DashboardInterfaces
{
    /// <summary>
    /// En çok talep alan ilan bilgisi.
    /// </summary>
    public class DashboardTopListing
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public string? ListingTitle { get; set; }
        public int LeadCount { get; set; }
    }

    /// <summary>
    /// Aylık işlem, ciro ve kapora trend noktası.
    /// </summary>
    public class DashboardMonthPoint
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int CompletedCount { get; set; }
        public decimal Commission { get; set; }
        public decimal Shares { get; set; }
        public int DepositCount { get; set; }
    }

    /// <summary>
    /// Danışman performans metrikleri (tutar alanları yalnızca Sahip/Yönetici yanıtında doldurulur).
    /// </summary>
    public class DashboardAgentPerformance
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int AssignedLeads { get; set; }
        public int WonLeads { get; set; }
        public int DealCount { get; set; }
        public decimal ShareTotal { get; set; }
        public decimal PaidTotal { get; set; }
        public decimal PendingTotal { get; set; }
    }

    /// <summary>
    /// Bu ayın kapora ve tamamlanan işlem özet sayı ve tutarları.
    /// </summary>
    public class DashboardMonthDeals
    {
        public int DepositCount { get; set; }
        public decimal DepositTotal { get; set; }
        public int CompletedCount { get; set; }
        public decimal CompletedCommission { get; set; }
    }

    /// <summary>
    /// Belirli aralıkta oluşturulan talepler ve randevu sayaçları.
    /// </summary>
    public class DashboardLeadStats
    {
        public int CreatedInRange { get; set; }
        public Dictionary<string, int> ByStatusInRange { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int UpcomingAppointments7Days { get; set; }
        public int TodayAppointments { get; set; }
    }
}
