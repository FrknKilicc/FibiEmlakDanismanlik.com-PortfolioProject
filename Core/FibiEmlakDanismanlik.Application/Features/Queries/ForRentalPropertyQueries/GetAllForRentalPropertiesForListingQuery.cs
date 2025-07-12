using FibiEmlakDanismanlik.Application.Features.Results.ForRentalResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries
{
    public class GetAllForRentalPropertiesForListingQuery:IRequest<List<GetAllForRentalPropertiesForListingResult>>
    {

    }
}
