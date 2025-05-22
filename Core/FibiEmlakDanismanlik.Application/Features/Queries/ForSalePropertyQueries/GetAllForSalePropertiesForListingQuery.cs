using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetAllForSalePropertiesForListingQuery:IRequest<List<GetAllForSalePropertiesForListingResult>>
    {
    }
}
