using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using FibiEmlakDanismanlik.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults
{
    public class ForSaleFilterResponseResult
    {
        public int Total { get; set; }
        public List<ForSalePropertyForListingViewModel> Items { get; set; } = new();
        public List<AmenityFacetResult> Amenties { get; set; } = new();
    }
}
