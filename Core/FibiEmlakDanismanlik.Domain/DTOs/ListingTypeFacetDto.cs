using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.DTOs
{
    public class ListingTypeFacetDto
    {
        public int ListingTypeId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
