using FibiEmlakDanismanlik.Application.Features.Results.ForSaleLandListingResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSaleLandListingQueries
{
    public class GetForSaleLandListingQuery:IRequest<List<GetForSalesLandListingResult>>
    {
    }
}
