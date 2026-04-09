using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class BlogCategory
    {
        [Key]
        public int BlogCategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}
