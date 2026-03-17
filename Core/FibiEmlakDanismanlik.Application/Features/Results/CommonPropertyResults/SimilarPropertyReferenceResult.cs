using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults
{
    public class SimilarPropertyReferenceResult
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
        public int UsageTypeId { get; set; }

        public string? ListingType { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }

        public decimal? Price { get; set; }
        public double? SquareMeter { get; set; }

        public string? NumberOfRoom { get; set; }
    }
}
