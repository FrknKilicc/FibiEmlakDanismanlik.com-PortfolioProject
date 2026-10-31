using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingDealResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class GetListingDealsQueryHandler : IRequestHandler<GetListingDealsQuery, GetListingDealsResult>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetListingDealsQueryHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetListingDealsResult> Handle(GetListingDealsQuery request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new DealAccessDeniedException();
            }

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize switch
            {
                < 1 => ListingDealConstants.DefaultPageSize,
                > ListingDealConstants.MaxPageSize => ListingDealConstants.MaxPageSize,
                _ => request.PageSize
            };

            request.Page = page;
            request.PageSize = pageSize;

            var (total, rows) = await _listingDealRepository.SearchAsync(request);
            var summary = await _listingDealRepository.GetSummaryAsync(request);

            return new GetListingDealsResult
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Rows = rows,
                Summary = summary
            };
        }
    }
}
