using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces.DashboardInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.DashboardRepositories
{
    /// <summary>
    /// Yönetim paneli kontrol paneli (dashboard) eksik toplam ve özet sorguları repository implementasyonu.
    /// </summary>
    public class DashboardRepository : IDashboardRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public DashboardRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<DashboardLeadStats> GetLeadStatsAsync(DateTime from, DateTime toExclusive, bool seeAll, int? agentId)
        {
            var contactsQuery = _context.customerContacts.AsNoTracking();
            if (!seeAll)
            {
                contactsQuery = contactsQuery.Where(c => c.AssignedAgentId == agentId);
            }

            var fibiQuery = _context.FibiServiceLeads.AsNoTracking();
            if (!seeAll)
            {
                fibiQuery = fibiQuery.Where(f => f.AssignedAgentId == agentId);
            }

            // Huni: aralıkta oluşturulan her talep, şu anki durumu ve geçmişte geçtiği tüm durumlar (LeadActivities) için bir kez sayılır
            var contactLeads = await contactsQuery
                .Where(c => c.CreatedDate >= from && c.CreatedDate < toExclusive)
                .Select(c => new { Id = c.CustomerContactId, Status = c.LeadStatus })
                .ToListAsync();

            var fibiLeads = await fibiQuery
                .Where(f => f.CreatedDate >= from && f.CreatedDate < toExclusive)
                .Select(f => new { Id = f.FibiServiceLeadId, Status = f.LeadStatus })
                .ToListAsync();

            var byStatus = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var createdInRange = contactLeads.Count + fibiLeads.Count;

            async Task CountReachedAsync(LeadSource source, List<(int Id, LeadStatus Status)> leads)
            {
                if (leads.Count == 0) return;
                var ids = leads.Select(l => l.Id).ToList();
                // shortcut: id listesi SQL IN olarak gider; aralıktaki talep sayısı binleri geçerse geçici tablo/join gerekir
                var history = await _context.LeadActivities.AsNoTracking()
                    .Where(a => a.Source == source && a.Type == LeadActivityType.StatusChange && a.NewStatus != null && ids.Contains(a.LeadId))
                    .Select(a => new { a.LeadId, NewStatus = a.NewStatus!.Value })
                    .ToListAsync();
                var historyByLead = history.ToLookup(h => h.LeadId, h => h.NewStatus);

                foreach (var lead in leads)
                {
                    var reached = new HashSet<LeadStatus> { LeadStatus.New, lead.Status };
                    reached.UnionWith(historyByLead[lead.Id]);
                    foreach (var status in reached)
                    {
                        var key = status.ToString();
                        byStatus[key] = byStatus.GetValueOrDefault(key) + 1;
                    }
                }
            }

            await CountReachedAsync(LeadSource.Contact, contactLeads.Select(l => (l.Id, l.Status)).ToList());
            await CountReachedAsync(LeadSource.FibiService, fibiLeads.Select(l => (l.Id, l.Status)).ToList());

            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1);
            var now = DateTime.Now;
            var next7DaysEnd = todayStart.AddDays(8);

            var apptQuery = contactsQuery
                .Where(c => c.AppointmentDateTime != null && c.LeadStatus != LeadStatus.Lost);

            var todayAppointments = await apptQuery
                .Where(c => c.AppointmentDateTime >= todayStart && c.AppointmentDateTime < todayEnd)
                .CountAsync();

            var upcomingAppointments7Days = await apptQuery
                .Where(c => c.AppointmentDateTime >= now && c.AppointmentDateTime < next7DaysEnd)
                .CountAsync();

            return new DashboardLeadStats
            {
                CreatedInRange = createdInRange,
                ByStatusInRange = byStatus,
                TodayAppointments = todayAppointments,
                UpcomingAppointments7Days = upcomingAppointments7Days
            };
        }

        public async Task<List<DashboardTopListing>> GetTopListingsByLeadsAsync(DateTime from, DateTime toExclusive, bool seeAll, int? agentId, int take)
        {
            var query = _context.customerContacts.AsNoTracking()
                .Where(c => c.PropertyListingId != null && c.PropertyListingTypeEnum != null)
                .Where(c => c.CreatedDate >= from && c.CreatedDate < toExclusive);

            if (!seeAll)
            {
                query = query.Where(c => c.AssignedAgentId == agentId);
            }

            var grouped = await query
                .GroupBy(c => new { c.PropertyListingTypeEnum, c.PropertyListingId })
                .Select(g => new
                {
                    Type = g.Key.PropertyListingTypeEnum,
                    Id = g.Key.PropertyListingId,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            var rows = grouped.Select(x => new DashboardTopListing
            {
                ListingTypeId = (int)x.Type!.Value,
                ListingId = x.Id!.Value,
                LeadCount = x.Count
            }).ToList();

            if (rows.Count > 0)
            {
                var refs = rows.Select(r => (r.ListingTypeId, r.ListingId)).ToList();
                var titleMap = await GetListingTitlesAsync(refs);
                foreach (var r in rows)
                {
                    if (titleMap.TryGetValue((r.ListingTypeId, r.ListingId), out var title))
                    {
                        r.ListingTitle = title;
                    }
                }
            }

            return rows;
        }

        public async Task<DashboardMonthDeals> GetMonthDealsAsync(DateTime monthStart)
        {
            var monthEnd = monthStart.AddMonths(1);
            var baseQuery = _context.ListingDeals.AsNoTracking();

            var depositStats = await baseQuery
                .Where(d => d.DepositDate != null && d.DepositDate >= monthStart && d.DepositDate < monthEnd)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Total = g.Sum(d => d.DepositAmount ?? 0m)
                })
                .FirstOrDefaultAsync();

            var completedStats = await baseQuery
                .Where(d => d.Status == DealStatus.Completed &&
                            d.CompletedDate != null &&
                            d.CompletedDate >= monthStart &&
                            d.CompletedDate < monthEnd)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    Total = g.Sum(d => (d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m))
                })
                .FirstOrDefaultAsync();

            return new DashboardMonthDeals
            {
                DepositCount = depositStats?.Count ?? 0,
                DepositTotal = depositStats?.Total ?? 0m,
                CompletedCount = completedStats?.Count ?? 0,
                CompletedCommission = completedStats?.Total ?? 0m
            };
        }

        public async Task<List<DashboardMonthPoint>> GetMonthlyDealsAsync(DateTime firstMonthStart, int months)
        {
            var rangeStart = new DateTime(firstMonthStart.Year, firstMonthStart.Month, 1);
            var rangeEnd = rangeStart.AddMonths(months);

            var completedDeals = await _context.ListingDeals.AsNoTracking()
                .Where(d => d.Status == DealStatus.Completed &&
                            d.CompletedDate != null &&
                            d.CompletedDate >= rangeStart &&
                            d.CompletedDate < rangeEnd)
                .Select(d => new
                {
                    Year = d.CompletedDate!.Value.Year,
                    Month = d.CompletedDate!.Value.Month,
                    Commission = (d.BuyerCommissionAmount ?? 0m) + (d.SellerCommissionAmount ?? 0m),
                    Shares = d.Shares.Sum(s => (decimal?)s.Amount) ?? 0m
                })
                .ToListAsync();

            var completedByMonth = completedDeals
                .GroupBy(d => (d.Year, d.Month))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Count = g.Count(),
                        Commission = g.Sum(x => x.Commission),
                        Shares = g.Sum(x => x.Shares)
                    }
                );

            var deposits = await _context.ListingDeals.AsNoTracking()
                .Where(d => d.DepositDate != null &&
                            d.DepositDate >= rangeStart &&
                            d.DepositDate < rangeEnd)
                .Select(d => new
                {
                    Year = d.DepositDate!.Value.Year,
                    Month = d.DepositDate!.Value.Month
                })
                .ToListAsync();

            var depositsByMonth = deposits
                .GroupBy(d => (d.Year, d.Month))
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<DashboardMonthPoint>(months);
            for (var i = 0; i < months; i++)
            {
                var current = rangeStart.AddMonths(i);
                var key = (current.Year, current.Month);

                var completed = completedByMonth.TryGetValue(key, out var cVal) ? cVal : null;
                var depositCount = depositsByMonth.TryGetValue(key, out var dVal) ? dVal : 0;

                result.Add(new DashboardMonthPoint
                {
                    Year = current.Year,
                    Month = current.Month,
                    CompletedCount = completed?.Count ?? 0,
                    Commission = completed?.Commission ?? 0m,
                    Shares = completed?.Shares ?? 0m,
                    DepositCount = depositCount
                });
            }

            return result;
        }

        public async Task<List<DashboardAgentPerformance>> GetAgentPerformanceAsync(DateTime from, DateTime toExclusive)
        {
            var activeAgents = await _context.Agents.AsNoTracking()
                .Where(a => a.IsActive)
                .Select(a => new { a.AgentId, a.AgentName })
                .ToListAsync();

            if (activeAgents.Count == 0)
            {
                return new List<DashboardAgentPerformance>();
            }

            var contacts = await _context.customerContacts.AsNoTracking()
                .Where(c => c.AssignedAgentId != null &&
                            c.CreatedDate >= from &&
                            c.CreatedDate < toExclusive)
                .Select(c => new
                {
                    AgentId = c.AssignedAgentId!.Value,
                    IsWon = c.LeadStatus == LeadStatus.Won
                })
                .ToListAsync();

            var fibiLeads = await _context.FibiServiceLeads.AsNoTracking()
                .Where(f => f.AssignedAgentId != null &&
                            f.CreatedDate >= from &&
                            f.CreatedDate < toExclusive)
                .Select(f => new
                {
                    AgentId = f.AssignedAgentId!.Value,
                    IsWon = f.LeadStatus == LeadStatus.Won
                })
                .ToListAsync();

            var leadsByAgent = contacts.Concat(fibiLeads)
                .GroupBy(x => x.AgentId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        AssignedCount = g.Count(),
                        WonCount = g.Count(x => x.IsWon)
                    }
                );

            var shares = await _context.DealShares.AsNoTracking()
                .Where(s => s.ListingDeal.Status == DealStatus.Completed &&
                            s.ListingDeal.CompletedDate != null &&
                            s.ListingDeal.CompletedDate >= from &&
                            s.ListingDeal.CompletedDate < toExclusive)
                .Select(s => new
                {
                    s.AgentId,
                    s.Amount,
                    s.PaymentStatus
                })
                .ToListAsync();

            var sharesByAgent = shares
                .GroupBy(s => s.AgentId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        DealCount = g.Count(),
                        ShareTotal = g.Sum(x => x.Amount),
                        PaidTotal = g.Where(x => x.PaymentStatus == DealSharePaymentStatus.Paid).Sum(x => x.Amount),
                        PendingTotal = g.Where(x => x.PaymentStatus == DealSharePaymentStatus.Pending).Sum(x => x.Amount)
                    }
                );

            var list = new List<DashboardAgentPerformance>();

            foreach (var agent in activeAgents)
            {
                var hasLead = leadsByAgent.TryGetValue(agent.AgentId, out var leadData);
                var hasShare = sharesByAgent.TryGetValue(agent.AgentId, out var shareData);

                var assignedLeads = hasLead ? leadData!.AssignedCount : 0;
                var wonLeads = hasLead ? leadData!.WonCount : 0;

                var dealCount = hasShare ? shareData!.DealCount : 0;
                var shareTotal = hasShare ? shareData!.ShareTotal : 0m;
                var paidTotal = hasShare ? shareData!.PaidTotal : 0m;
                var pendingTotal = hasShare ? shareData!.PendingTotal : 0m;

                if (assignedLeads == 0 && dealCount == 0)
                {
                    continue;
                }

                list.Add(new DashboardAgentPerformance
                {
                    AgentId = agent.AgentId,
                    AgentName = agent.AgentName,
                    AssignedLeads = assignedLeads,
                    WonLeads = wonLeads,
                    DealCount = dealCount,
                    ShareTotal = shareTotal,
                    PaidTotal = paidTotal,
                    PendingTotal = pendingTotal
                });
            }

            return list
                .OrderByDescending(x => x.ShareTotal)
                .ThenByDescending(x => x.WonLeads)
                .ThenBy(x => x.AgentName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        #region Private Helpers

        private async Task<Dictionary<(int TypeId, int Id), string>> GetListingTitlesAsync(List<(int TypeId, int Id)> refs)
        {
            var dict = new Dictionary<(int TypeId, int Id), string>();
            if (refs.Count == 0) return dict;

            var landSaleIds = refs.Where(r => r.TypeId == 1).Select(r => r.Id).ToList();
            if (landSaleIds.Count > 0)
            {
                var items = await _context.forSaleLandListings.AsNoTracking()
                    .Where(l => landSaleIds.Contains(l.ForSaleLandListingId))
                    .Select(l => new { l.ForSaleLandListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(1, it.ForSaleLandListingId)] = it.PropertyName;
            }

            var houseSaleIds = refs.Where(r => r.TypeId == 2).Select(r => r.Id).ToList();
            if (houseSaleIds.Count > 0)
            {
                var items = await _context.forSaleHousingPropertyListings.AsNoTracking()
                    .Where(l => houseSaleIds.Contains(l.ForSaleHousingListId))
                    .Select(l => new { l.ForSaleHousingListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(2, it.ForSaleHousingListId)] = it.PropertyName;
            }

            var commSaleIds = refs.Where(r => r.TypeId == 3).Select(r => r.Id).ToList();
            if (commSaleIds.Count > 0)
            {
                var items = await _context.forSaleCommercialPropertyListings.AsNoTracking()
                    .Where(l => commSaleIds.Contains(l.ForSaleCommercialListingId))
                    .Select(l => new { l.ForSaleCommercialListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(3, it.ForSaleCommercialListingId)] = it.PropertyName;
            }

            var landRentIds = refs.Where(r => r.TypeId == 7).Select(r => r.Id).ToList();
            if (landRentIds.Count > 0)
            {
                var items = await _context.rentalLandListings.AsNoTracking()
                    .Where(l => landRentIds.Contains(l.RentalLandListingId))
                    .Select(l => new { l.RentalLandListingId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(7, it.RentalLandListingId)] = it.PropertyName;
            }

            var houseRentIds = refs.Where(r => r.TypeId == 8).Select(r => r.Id).ToList();
            if (houseRentIds.Count > 0)
            {
                var items = await _context.rentalHousingListings.AsNoTracking()
                    .Where(l => houseRentIds.Contains(l.RentalHousingListId))
                    .Select(l => new { l.RentalHousingListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(8, it.RentalHousingListId)] = it.PropertyName;
            }

            var commRentIds = refs.Where(r => r.TypeId == 9).Select(r => r.Id).ToList();
            if (commRentIds.Count > 0)
            {
                var items = await _context.rentalCommercialPropertyListings.AsNoTracking()
                    .Where(l => commRentIds.Contains(l.RentalCommercialListId))
                    .Select(l => new { l.RentalCommercialListId, l.PropertyName })
                    .ToListAsync();
                foreach (var it in items) dict[(9, it.RentalCommercialListId)] = it.PropertyName;
            }

            return dict;
        }

        #endregion
    }
}
