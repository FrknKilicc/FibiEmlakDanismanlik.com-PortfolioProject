using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class ForSaleFilterResponseDto
    {
        public int Total { get; set; }
        public List<ResultAllForSaleListinForPageDto>  Items { get; set; }
        public List<AmenityFacetDto> Amenties { get; set; } = new();
    }
}
