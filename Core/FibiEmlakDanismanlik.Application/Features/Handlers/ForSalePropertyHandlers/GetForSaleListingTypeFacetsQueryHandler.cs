using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetForSaleListingTypeFacetsQueryHandler : IRequestHandler<GetForSaleListingTypeFacetsQuery, List<ListingTypeFacetDto>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetForSaleListingTypeFacetsQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<ListingTypeFacetDto>> Handle(GetForSaleListingTypeFacetsQuery request, CancellationToken cancellationToken)
        {
            return await _propertyRepository.GetForSaleListingTypeFacetsAsync();
        }
    }
}
