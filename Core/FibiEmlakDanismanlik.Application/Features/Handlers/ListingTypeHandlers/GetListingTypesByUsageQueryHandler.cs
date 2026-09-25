using FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingTypeHandlers
{
    public class GetListingTypesByUsageQueryHandler
        : IRequestHandler<GetListingTypesByUsageQuery, List<ListingTypeResult>>
    {
        private readonly IListingTypeRepository _listingTypeRepository;

        public GetListingTypesByUsageQueryHandler(IListingTypeRepository listingTypeRepository)
        {
            _listingTypeRepository = listingTypeRepository;
        }

        public Task<List<ListingTypeResult>> Handle(
            GetListingTypesByUsageQuery request,
            CancellationToken cancellationToken)
        {
            return _listingTypeRepository.GetByUsageTypeAsync(request.UsageType);
        }
    }
}

