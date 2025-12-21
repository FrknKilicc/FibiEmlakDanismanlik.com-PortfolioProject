using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.BlogDtos
{
    public class BlogSuggestionDto
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogDescPrew { get; set; }
        public string BlogImgUrl { get; set; }
    }
}
