using FibiEmlakDanismanlik.Application.Features.Results.ForSaleLandListingResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSaleLandListingQueries
{
    public class GetForSaleLandListingByIdQuery:IRequest<GetForSalesLandListingByIdResult>
    {
        public int Id { get; set; }

        public GetForSaleLandListingByIdQuery(int id)
        {
            Id = id;
        }
    }
}
