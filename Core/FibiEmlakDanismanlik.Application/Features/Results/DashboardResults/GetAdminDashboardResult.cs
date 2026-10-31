using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.DashboardInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.DashboardResults
{
    /// <summary>
    /// Yönetim paneli kontrol paneli (dashboard) birleşik sonuç modeli.
    /// </summary>
    public class GetAdminDashboardResult
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool CanSeeFinance { get; set; }
        public AdminListingSummary Listings { get; set; } = null!;
        public LeadSummary Leads { get; set; } = null!;
        public DashboardLeadStats LeadStats { get; set; } = null!;
        public ListingAlertSummary Alerts { get; set; } = null!;
        public List<DashboardTopListing> TopListings { get; set; } = new();
        public DashboardFinance? Finance { get; set; }
        public List<DashboardAgentPerformance> Agents { get; set; } = new();
    }

    /// <summary>
    /// Yöneticilere özel finansal işlem ve ciro verileri.
    /// </summary>
    public class DashboardFinance
    {
        public DashboardMonthDeals ThisMonth { get; set; } = null!;
        public ListingDealSummary Range { get; set; } = null!;
        public List<DashboardMonthPoint> Monthly { get; set; } = new();
    }
}
