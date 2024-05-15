using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForSaleCommercialPropertyListingResults
{
    public class GetForSaleCommercialPropertyListingByIdResult
    {
        public int Id { get; set; }

        public GetForSaleCommercialPropertyListingByIdResult(int id)
        {
            Id = id;
        }
    }
}
