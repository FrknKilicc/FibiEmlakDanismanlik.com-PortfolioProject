using FibiEmlakDanismanlik.Application.Features.Results.ForSaleHousingListingResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSaleHousingListingQueries
{
    public class GetForSaleHousingPropertyListingQuery:IRequest<List<GetForSaleHousingListingResult>>
    {

    }
}
