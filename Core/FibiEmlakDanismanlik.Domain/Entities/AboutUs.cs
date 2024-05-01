using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class AboutUs
    {
        [Key]
        public int AboutUsId { get; set; }
        public string AboutUsTitle { get; set; }
        public string AboutUsDesc { get; set; }
        public string AboutUsImgUrl { get; set; }
    }
}
