using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class HousingCategory
    {
        [Key]
        public int HousingCategoryId { get; set; }
        public string HousingCategoryName { get; set; }
   
    }
}
