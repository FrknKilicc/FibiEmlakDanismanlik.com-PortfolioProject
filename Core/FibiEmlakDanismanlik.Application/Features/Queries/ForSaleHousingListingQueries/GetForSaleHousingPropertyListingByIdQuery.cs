using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSaleHousingListingQueries
{
    public class GetForSaleHousingPropertyListingByIdQuery:IRequest
    {
        public int Id { get; set; }

        public GetForSaleHousingPropertyListingByIdQuery(int ıd)
        {
            Id = ıd;
        }
    }
}
