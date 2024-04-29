using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class MainCategory
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }


    }
}
