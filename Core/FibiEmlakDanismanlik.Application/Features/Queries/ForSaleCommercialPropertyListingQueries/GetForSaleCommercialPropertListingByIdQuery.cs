using FibiEmlakDanismanlik.Application.Features.Results.ForSaleCommercialPropertyListingResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries
{
    public class GetForSaleCommercialPropertListingByIdQuery:IRequest<GetForSaleCommercialPropertyListingByIdResult>
    {
        public int Id { get; set; }

        public GetForSaleCommercialPropertListingByIdQuery(int id)
        {
            Id = id;
        }
    }
}
