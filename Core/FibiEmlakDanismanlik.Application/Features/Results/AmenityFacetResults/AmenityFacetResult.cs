using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults
{
    public class AmenityFacetResult
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public string? Group { get; set; }          // UI gruplama 
        public int SortOrder { get; set; }          // UI sıralama

        public int Count { get; set; }              // kaç ilan
        public bool Selected { get; set; }          // şu an seçili mi
        public bool Disabled { get; set; }          // Count==0 ise true
    }
}
