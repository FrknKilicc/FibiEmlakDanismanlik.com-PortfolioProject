using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults
{
    public class RentalListingFilterResponseResult
    {
        public int Total { get; set; }

        public List<RentalListingBaseResult> Items { get; set; } = new();

        public List<AmenityFacetResult> Amenties { get; set; } = new();
    }
}
