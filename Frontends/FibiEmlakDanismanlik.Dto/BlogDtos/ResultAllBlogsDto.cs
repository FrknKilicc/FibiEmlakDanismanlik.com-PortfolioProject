using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.BlogDtos
{
    public class ResultAllBlogsDto
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogDescription { get; set; }
        public string BlogImgUrl { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool TopNews { get; set; }
        public int AuthorId { get; set; }
    }
}
