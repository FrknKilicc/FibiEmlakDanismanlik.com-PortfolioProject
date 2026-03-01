using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public   class RentalHousingListingAmenities
    {
        public int Id { get; set; }

        public int RentalHousingListId { get; set; }

        [ForeignKey(nameof(RentalHousingListId))]
        public RentalHousingListing RentalHousingListing { get; set; }

        public int AmenityId { get; set; }

        [ForeignKey(nameof(AmenityId))]
        public AmenityDefinition AmenityDefinition { get; set; }

    }
}
