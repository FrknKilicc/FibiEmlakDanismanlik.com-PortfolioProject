using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class SimilarPropertyItemDto
    {
        public int ListingId { get; set; }

        public string? ListingType { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }

        public string? AgentName { get; set; }
        public string? AgentImageUrl { get; set; }
        public string? PropertyStatus { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }

        public decimal? Price { get; set; }
        public double? SquareMeter { get; set; }
        public string? NumberOfRoom { get; set; }

        public string? CoverImageUrl { get; set; }
    }
}
