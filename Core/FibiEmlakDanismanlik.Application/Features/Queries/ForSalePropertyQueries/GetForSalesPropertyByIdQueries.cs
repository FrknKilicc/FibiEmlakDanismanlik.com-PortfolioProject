using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetForSalesPropertyByIdQueries:IRequest<GetAllForSalePropertiesForListingResult>
    {
        public int Id { get; set; }

        public GetForSalesPropertyByIdQueries(int id)
        {
            Id = id;
        }
    }
}
