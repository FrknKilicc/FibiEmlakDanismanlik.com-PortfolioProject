using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.DashboardInterfaces
{
    /// <summary>
    /// Yönetim paneli kontrol paneli (dashboard) eksik toplam ve özet sorguları repository arayüzü.
    /// </summary>
    public interface IDashboardRepository
    {
        Task<DashboardLeadStats> GetLeadStatsAsync(DateTime from, DateTime toExclusive, bool seeAll, int? agentId);
        Task<List<DashboardTopListing>> GetTopListingsByLeadsAsync(DateTime from, DateTime toExclusive, bool seeAll, int? agentId, int take);
        Task<DashboardMonthDeals> GetMonthDealsAsync(DateTime monthStart);
        Task<List<DashboardMonthPoint>> GetMonthlyDealsAsync(DateTime firstMonthStart, int months);
        Task<List<DashboardAgentPerformance>> GetAgentPerformanceAsync(DateTime from, DateTime toExclusive);
    }
}
