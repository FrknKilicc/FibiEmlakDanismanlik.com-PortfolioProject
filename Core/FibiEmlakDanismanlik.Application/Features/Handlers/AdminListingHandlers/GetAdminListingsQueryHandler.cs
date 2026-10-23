using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Yönetim paneli ilan listeleme ve özet sorgusunu işleyen işleyici.
    /// </summary>
    public class GetAdminListingsQueryHandler : IRequestHandler<GetAdminListingsQuery, GetAdminListingsResult>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAdminListingsQueryHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository,
            ICurrentUserService currentUserService)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetAdminListingsResult> Handle(GetAdminListingsQuery request, CancellationToken cancellationToken)
        {
            var scope = await _listingAccessService.GetScopeAsync();

            var filter = new AdminListingFilter
            {
                IsRent = request.IsRent,
                Category = request.Category,
                Status = request.Status,
                AgentId = request.AgentId,
                Search = request.Search,
                Page = request.Page,
                PageSize = request.PageSize,
                Sort = request.Sort
            };

            var (total, rows) = await _adminListingRepository.SearchAsync(filter, scope);
            var summary = await _adminListingRepository.GetSummaryAsync(scope);

            // Satır bazında düzenleme yetkisi (CanEdit) belirleme
            var isManagerOrAbove = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;

            if (isManagerOrAbove)
            {
                foreach (var row in rows)
                {
                    row.CanEdit = true;
                }
            }
            else if (_currentUserService.AgentId.HasValue)
            {
                var currentAgentId = _currentUserService.AgentId.Value;
                var grants = await _adminListingRepository.GetGrantsForAgentAsync(currentAgentId);
                var editGrants = grants
                    .Where(g => g.Access == ListingAccessLevel.Edit || (int)g.Access == 2)
                    .Select(g => (g.ListingTypeId, g.ListingId))
                    .ToHashSet();

                foreach (var row in rows)
                {
                    row.CanEdit = (row.AgentId == currentAgentId) || editGrants.Contains((row.ListingTypeId, row.ListingId));
                }
            }

            return new GetAdminListingsResult
            {
                Total = total,
                Page = filter.Page < 1 ? 1 : filter.Page,
                PageSize = filter.PageSize,
                Items = rows,
                Summary = summary
            };
        }
    }
}
