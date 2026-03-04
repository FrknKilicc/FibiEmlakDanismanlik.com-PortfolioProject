using FibiEmlakDanismanlik.Application.Features.Queries.MapQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MapHandlers
{
    public class GetListingMarkerQueryHandler : IRequestHandler<GetListingMarkerQuery, ListingMarkerResult>
    {
        private readonly IMapRepository mapRepository;

        public GetListingMarkerQueryHandler(IMapRepository mapRepository)
        {
            this.mapRepository = mapRepository;
        }

        public Task<ListingMarkerResult?> Handle(GetListingMarkerQuery request, CancellationToken cancellationToken)
            => mapRepository.GetListingMarkerAsync(request.ListingId, request.ListingTypeId);
    }
}
