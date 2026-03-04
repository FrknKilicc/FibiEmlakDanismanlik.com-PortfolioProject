using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ListingGeoPoint
    {
        [Key]
        public int ListingGeoPointId { get; set; }

        // Listing’in kendi tablosundaki Id (ForSaleHousingListId / RentalHousingListId vs)
        public int ListingId { get; set; }

        // ListingType tablosundaki Id
        public int ListingTypeId { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        // Manual / Geocode gibi
        public string? Source { get; set; }

        // istersen 1-2-3 gibi seviyeler: GPS, rooftop, district-center vs
        public string? Precision { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
