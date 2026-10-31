using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    /// <summary>
    /// Belirli bir ilana ait kriterlere uyan aktif haber taleplerini ve kapsam dışı kayıt sayısını getiren sorgu işleyicisi.
    /// </summary>
    public class GetListingAlertsForListingQueryHandler : IRequestHandler<GetListingAlertsForListingQuery, GetListingAlertsForListingResult>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly IListingAccessService _listingAccessService;
        private readonly ICurrentUserService _currentUserService;

        public GetListingAlertsForListingQueryHandler(
            IListingAlertRepository listingAlertRepository,
            IListingAccessService listingAccessService,
            ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _listingAccessService = listingAccessService;
            _currentUserService = currentUserService;
        }

        public async Task<GetListingAlertsForListingResult> Handle(GetListingAlertsForListingQuery request, CancellationToken cancellationToken)
        {
            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access == ListingAccess.None)
            {
                throw new ListingAccessDeniedException("Bu ilanın haber bekleyenlerini görüntüleme yetkiniz yok.");
            }

            var seeAll = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            var scope = new ListingAlertScope
            {
                SeeAll = seeAll,
                AgentId = _currentUserService.AgentId
            };

            var items = await _listingAlertRepository.GetAlertsForListingAsync(request.ListingTypeId, request.ListingId, scope);
            var otherCount = await _listingAlertRepository.CountAlertsForListingOutsideScopeAsync(request.ListingTypeId, request.ListingId, scope);

            return new GetListingAlertsForListingResult
            {
                Items = items,
                OtherCount = otherCount
            };
        }
    }
}
