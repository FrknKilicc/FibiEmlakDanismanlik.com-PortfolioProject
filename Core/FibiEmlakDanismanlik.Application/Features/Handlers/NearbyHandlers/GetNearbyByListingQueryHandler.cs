using FibiEmlakDanismanlik.Application.Features.Queries.NearbyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.NearbyResults;
using FibiEmlakDanismanlik.Application.Interfaces.NearbyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.NearbyHandlers
{
    public class GetNearbyByListingQueryHandler : IRequestHandler<GetNearbyByListingQuery, List<NearbyCategoryGroupResult>>
    {
        private readonly INearbyRepository _nearbyRepository;

        public GetNearbyByListingQueryHandler(INearbyRepository nearbyRepository)
        {
            _nearbyRepository = nearbyRepository;
        }

        public async Task<List<NearbyCategoryGroupResult>> Handle(GetNearbyByListingQuery request, CancellationToken cancellationToken)
        {
            if (request.ListingId <= 0 || request.ListingType <= 0)
                return new List<NearbyCategoryGroupResult>();

            return await _nearbyRepository.GetNearbyByListingAsync(request.ListingId, request.ListingType);
        }
    }
}
