using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Testimonials
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Testimonail { get; set; }

    }
}
