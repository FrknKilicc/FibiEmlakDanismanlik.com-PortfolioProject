using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.DashboardQueries;
using FibiEmlakDanismanlik.Application.Features.Results.DashboardResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.DashboardInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.DashboardHandlers
{
    /// <summary>
    /// Yönetim paneli kontrol paneli (dashboard) verilerini toplayıp kullanıcı rolüne göre filtreleyen sorgu işleyicisi.
    /// </summary>
    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, GetAdminDashboardResult>
    {
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly IListingAccessService _listingAccessService;
        private readonly ILeadRepository _leadRepository;
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly IListingDealRepository _listingDealRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAdminDashboardQueryHandler(
            IAdminListingRepository adminListingRepository,
            IListingAccessService listingAccessService,
            ILeadRepository leadRepository,
            IListingAlertRepository listingAlertRepository,
            IListingDealRepository listingDealRepository,
            IDashboardRepository dashboardRepository,
            ICurrentUserService currentUserService)
        {
            _adminListingRepository = adminListingRepository;
            _listingAccessService = listingAccessService;
            _leadRepository = leadRepository;
            _listingAlertRepository = listingAlertRepository;
            _listingDealRepository = listingDealRepository;
            _dashboardRepository = dashboardRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetAdminDashboardResult> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var to = request.To?.Date ?? today;
            var from = request.From?.Date ?? new DateTime(today.Year, 1, 1);

            if (from > to)
            {
                (from, to) = (to, from);
            }

            if (from < to.AddYears(-3))
            {
                from = to.AddYears(-3);
            }

            var toExclusive = to.Date.AddDays(1);

            var seeAll = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            var currentAgentId = _currentUserService.AgentId;

            var listingScope = await _listingAccessService.GetScopeAsync();
            var listings = await _adminListingRepository.GetSummaryAsync(listingScope);

            var leadScope = new LeadScope
            {
                SeeAll = seeAll,
                AgentId = currentAgentId
            };
            var leads = await _leadRepository.GetSummaryAsync(leadScope);

            var alertScope = new ListingAlertScope
            {
                SeeAll = seeAll,
                AgentId = currentAgentId
            };
            var alerts = await _listingAlertRepository.GetSummaryAsync(alertScope);

            var leadStats = await _dashboardRepository.GetLeadStatsAsync(from, toExclusive, seeAll, currentAgentId);
            var topListings = await _dashboardRepository.GetTopListingsByLeadsAsync(from, toExclusive, seeAll, currentAgentId, 5);

            DashboardFinance? finance = null;
            if (seeAll)
            {
                var thisMonthStart = new DateTime(today.Year, today.Month, 1);
                var thisMonthDeals = await _dashboardRepository.GetMonthDealsAsync(thisMonthStart);

                var rangeDeals = await _listingDealRepository.GetSummaryAsync(new ListingDealFilter
                {
                    From = from,
                    To = to
                });

                var firstMonthStart = new DateTime(to.Year, to.Month, 1).AddMonths(-11);
                var monthlyDeals = await _dashboardRepository.GetMonthlyDealsAsync(firstMonthStart, 12);

                finance = new DashboardFinance
                {
                    ThisMonth = thisMonthDeals,
                    Range = rangeDeals,
                    Monthly = monthlyDeals
                };
            }

            var agents = await _dashboardRepository.GetAgentPerformanceAsync(from, toExclusive);

            if (!seeAll)
            {
                if (currentAgentId.HasValue)
                {
                    var myAgent = agents.FirstOrDefault(a => a.AgentId == currentAgentId.Value);
                    if (myAgent != null)
                    {
                        myAgent.ShareTotal = 0m;
                        myAgent.PaidTotal = 0m;
                        myAgent.PendingTotal = 0m;
                        agents = new List<DashboardAgentPerformance> { myAgent };
                    }
                    else
                    {
                        agents = new List<DashboardAgentPerformance>();
                    }
                }
                else
                {
                    agents = new List<DashboardAgentPerformance>();
                }
            }

            return new GetAdminDashboardResult
            {
                From = from,
                To = to,
                CanSeeFinance = seeAll,
                Listings = listings,
                Leads = leads,
                LeadStats = leadStats,
                Alerts = alerts,
                TopListings = topListings,
                Finance = finance,
                Agents = agents
            };
        }
    }
}
