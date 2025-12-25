using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetFilteredForSalePropertiesForListingQuery:IRequest<List<GetAllForSalePropertiesForListingResult>>
    {
        public PropertyFilterRequest Filter { get;}

        public GetFilteredForSalePropertiesForListingQuery(PropertyFilterRequest filter)
        {
            Filter = filter;
        }
    }
}
