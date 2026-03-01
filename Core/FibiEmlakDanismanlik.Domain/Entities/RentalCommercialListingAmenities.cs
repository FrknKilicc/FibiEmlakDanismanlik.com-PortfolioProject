using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class RentalCommercialListingAmenities
    {
        public int Id { get; set; }

        public int RentalCommercialListId { get; set; }

        [ForeignKey(nameof(RentalCommercialListId))]
        public RentalCommercialPropertyListing RentalCommercialPropertyListing { get; set; }

        public int AmenityId { get; set; }

        [ForeignKey(nameof(AmenityId))]
        public AmenityDefinition AmenityDefinition { get; set; }
    }
}
