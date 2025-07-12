using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetAllForRentalPropertiesForListingQueryHandler : IRequestHandler<GetAllForRentalPropertiesForListingQuery, List<GetAllForRentalPropertiesForListingResult>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetAllForRentalPropertiesForListingQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<GetAllForRentalPropertiesForListingResult>> Handle(GetAllForRentalPropertiesForListingQuery request, CancellationToken cancellationToken)
        {
            
        }
    }
}
