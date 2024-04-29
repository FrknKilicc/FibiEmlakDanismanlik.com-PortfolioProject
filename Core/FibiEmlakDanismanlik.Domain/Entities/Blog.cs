using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogDescription { get; set; }
        public string BlogImgUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }

    }
}
