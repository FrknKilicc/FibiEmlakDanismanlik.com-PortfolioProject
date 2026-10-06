using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests
{
    public class PropertyFilterRequest
    {

        public List<int>? ListingTypeIds { get; set; } 

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? NumberOfRoom { get; set; }

       
        public int Page { get; set; }      
        public int PageSize { get; set; } 
        public string? SortBy { get; set; } = "CreatedDate"; 
        public string? SortDir { get; set; } = "desc";

        public List<int>? SelectedAmenityIds{ get; set; }

        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public int? NumberOfBathRoom { get; set; }
        public int? Floor { get; set; }
        public int? NumberOfSection { get; set; }
        public int? AgentId { get; set; }
        public string? ZoningStatus { get; set; }
        public string? SearchTerm { get; set; }

    }
    }