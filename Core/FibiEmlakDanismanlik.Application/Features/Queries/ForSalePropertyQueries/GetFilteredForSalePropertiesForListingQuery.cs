using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using MediatR;


namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetFilteredForSalePropertiesForListingQuery:IRequest<ForSaleFilterResponseResult>
    {
        public PropertyFilterRequest Filter { get;}

        public GetFilteredForSalePropertiesForListingQuery(PropertyFilterRequest filter)
        {
            Filter = filter;
        }
    }

}
