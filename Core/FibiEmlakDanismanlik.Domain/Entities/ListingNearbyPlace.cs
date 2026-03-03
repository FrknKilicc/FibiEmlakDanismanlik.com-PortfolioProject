using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ListingNearbyPlace
    {
        public int Id { get; set; }

        // ilanı tekilleştiren ikili
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; } 

        public int NearbyCategoryId { get; set; }
        public NearbyCategory? NearbyCategory { get; set; }

        public string PlaceName { get; set; } = null!;

        public decimal DistanceKm { get; set; } //
        public byte? Stars { get; set; } 

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
