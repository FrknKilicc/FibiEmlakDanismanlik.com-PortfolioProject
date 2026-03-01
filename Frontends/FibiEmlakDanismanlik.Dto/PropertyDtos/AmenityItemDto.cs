using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class AmenityItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public string? Group { get; set; }         
        public int SortOrder { get; set; }
    }
}
