using FibiEmlakDanismanlik.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetForSaleListingTypeFacetsQuery:IRequest<List<ListingTypeFacetDto>>
    {

    }
}
