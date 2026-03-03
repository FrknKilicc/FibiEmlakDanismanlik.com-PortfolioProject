using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.NearbyResults
{
    public class NearbyPlaceItemResult
    {
        public string PlaceName { get; set; } = null!;
        public decimal DistanceKm { get; set; }
        public byte? Stars { get; set; }
    }
}
