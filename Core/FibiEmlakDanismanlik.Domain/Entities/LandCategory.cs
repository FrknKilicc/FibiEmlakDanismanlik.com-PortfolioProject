using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class LandCategory
    {
        public int LandCategoryId { get; set; }
        public string LandCategoryName { get; set; }
        public int MainCategoryId { get; set; }
        public MainCategory MainCategory { get; set; }
    }
}
