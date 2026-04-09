using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.BlogDtos
{
    public class BlogCategoryCountDto
    {
        public int? BlogCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? BlogCount { get; set; }
    }
}
