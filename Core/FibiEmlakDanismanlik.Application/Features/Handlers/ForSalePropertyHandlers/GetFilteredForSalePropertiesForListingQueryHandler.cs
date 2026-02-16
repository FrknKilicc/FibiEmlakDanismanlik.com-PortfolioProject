using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetFilteredForSalePropertiesForListingQueryHandler : IRequestHandler<GetFilteredForSalePropertiesForListingQuery, ForSaleFilterResponseResult>
    {
        private readonly IPropertyRepository _propertyRepository;


        public GetFilteredForSalePropertiesForListingQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<ForSaleFilterResponseResult> Handle(
            GetFilteredForSalePropertiesForListingQuery request,
            CancellationToken cancellationToken)
        {
            return await _propertyRepository
                .GetFilteredForSalePropertyForListingWithFacets(request.Filter);
        }
    }
}

