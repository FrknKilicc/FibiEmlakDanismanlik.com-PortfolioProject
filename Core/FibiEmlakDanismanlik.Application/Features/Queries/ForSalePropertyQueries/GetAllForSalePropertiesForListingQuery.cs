using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Domain.DTOs;
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
        public ForSaleListingFilterDto Filter { get; }

        public GetAllForSalePropertiesForListingQuery(ForSaleListingFilterDto filter)
        {
            Filter = filter ?? new ForSaleListingFilterDto();
        }
    }
}
