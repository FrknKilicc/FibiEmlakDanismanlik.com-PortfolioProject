using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.NearbyResults
{
    public class NearbyCategoryGroupResult
    {
        public string CategoryName { get; set; } = null!;
        public string? IconCss { get; set; }
        public int SortOrder { get; set; }
        public List<NearbyPlaceItemResult> Items { get; set; } = new();
        
    }
}
