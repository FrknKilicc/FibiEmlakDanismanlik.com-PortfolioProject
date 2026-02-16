using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleHousingListingAmenity
    {
        public int ForSaleHousingListId { get; set; }
        public ForSaleHousingListing forSaleHousingListing { get; set; }

        public int AmenityId { get; set; }
        public AmenityDefinition AmenityDefinition  { get; set; }
    }
}
