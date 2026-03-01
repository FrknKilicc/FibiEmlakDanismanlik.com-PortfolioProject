using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetForRentalListingTypeFacetsQueryHandler : IRequestHandler<GetForRentalListingTypeFacetsQuery, List<ListingTypeFacetDto>>
    {
        private readonly IPropertyRepository _repository;

        public GetForRentalListingTypeFacetsQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ListingTypeFacetDto>> Handle(GetForRentalListingTypeFacetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetForRentalListingTypeFacetsAsync();
        }
    }
}
