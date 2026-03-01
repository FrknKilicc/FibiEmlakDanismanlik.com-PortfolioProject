using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetFilteredForRentalPropertiesForListingQueryHandler : IRequestHandler<GetFilteredForRentalPropertiesForListingQuery, RentalListingFilterResponseResult>
    {
        private readonly IPropertyRepository _repository;

        public GetFilteredForRentalPropertiesForListingQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<RentalListingFilterResponseResult> Handle(GetFilteredForRentalPropertiesForListingQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetFilteredForRentalPropertyForListingWithFacets(request.Filter);
        }
    }
}
