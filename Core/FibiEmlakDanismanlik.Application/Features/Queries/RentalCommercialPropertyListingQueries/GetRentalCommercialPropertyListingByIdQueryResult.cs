using FibiEmlakDanismanlik.Application.Features.Results.RentalCommercialPropertyListingResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.RentalCommercialPropertyListingQueries
{
    public class GetRentalCommercialPropertyListingByIdQueryResult:IRequest<GetRentalCommercialPropertyListingByIdResult>
    {
        public int Id { get; set; }

        public GetRentalCommercialPropertyListingByIdQueryResult(int id)
        {
            Id = id;
        }
    }
}
