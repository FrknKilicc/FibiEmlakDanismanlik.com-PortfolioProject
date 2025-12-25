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

       
        public int Page { get; set; } = 1;        
        public int PageSize { get; set; } = 20;  
        public string? SortBy { get; set; } = "CreatedDate"; 
        public string? SortDir { get; set; } = "desc";      
    }
    }
