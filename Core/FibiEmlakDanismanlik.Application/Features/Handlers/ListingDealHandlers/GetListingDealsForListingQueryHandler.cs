using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class GetListingDealsForListingQueryHandler : IRequestHandler<GetListingDealsForListingQuery, List<ListingDealRow>>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetListingDealsForListingQueryHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ListingDealRow>> Handle(GetListingDealsForListingQuery request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new DealAccessDeniedException();
            }

            return await _listingDealRepository.GetForListingAsync(request.ListingTypeId, request.ListingId);
        }
    }
}
