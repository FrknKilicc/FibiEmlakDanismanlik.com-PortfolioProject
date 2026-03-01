using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults
{
    public abstract class RentalListingBaseResult
    {
        public string ItemType { get; set; } = "";

        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
        public int UsageTypeId { get; set; } // ForRent : 7-89 olabilir

        public int? PropertyNo { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public decimal? Price { get; set; }

        public decimal? Deposit { get; set; }

        public bool? BestDeals { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? AddressDesc { get; set; }

        public int? AgentId { get; set; }
        public string? AgentName { get; set; }
        public string? AgentTitle { get; set; }
        public string? AgentImgUrl { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        public List<string> Highlights { get; set; } = new();
    }
}
