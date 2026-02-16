using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleCommercialListingAmenity
    {
        public int ForSaleCommercialListingId { get; set; }
        public ForSaleCommercialPropertyListing ForSaleCommercialProperty { get; set; }

        public int AmenityId { get; set; }
        public AmenityDefinition AmenityDefinition { get; set; }

    }
}
