using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    /// <summary>
    /// Haber bekleyen talepleri kullanıcı rol ve yetki kapsamına göre filtreleyen, arayan ve özet sayaçlarını üreten sorgu işleyicisi.
    /// </summary>
    public class GetListingAlertsQueryHandler : IRequestHandler<GetListingAlertsQuery, GetListingAlertsResult>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetListingAlertsQueryHandler(IListingAlertRepository listingAlertRepository, ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetListingAlertsResult> Handle(GetListingAlertsQuery request, CancellationToken cancellationToken)
        {
            var seeAll = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            var scope = new ListingAlertScope
            {
                SeeAll = seeAll,
                AgentId = _currentUserService.AgentId
            };

            var (total, rows) = await _listingAlertRepository.SearchAsync(request, scope);
            var summary = await _listingAlertRepository.GetSummaryAsync(scope);

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? AdminListingConstants.DefaultPageSize : request.PageSize;

            return new GetListingAlertsResult
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = rows,
                Summary = summary
            };
        }
    }
}
