using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.MapResults
{
    public class ListingMarkerResult
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public int Zoom { get; set; } = 15;

        public string Title { get; set; } = "";
        public string? Subtitle { get; set; }

        public string IconKey { get; set; } = "default";
        public string? PropertyStatus { get; set; }

        public int UsageTypeId { get; set; }          // 1 Satılık, 2 Kiralık, 3 Both
        public string ListingTypeName { get; set; } = "";
    }
}
