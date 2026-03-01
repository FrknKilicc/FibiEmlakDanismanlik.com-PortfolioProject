using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries
{
    public class GetFilteredForRentalPropertiesForListingQuery : IRequest<RentalListingFilterResponseResult>
    {
        public PropertyFilterRequest Filter { get; }

        public GetFilteredForRentalPropertiesForListingQuery (PropertyFilterRequest filter)
        {
            Filter = filter;
        }

    }
}
