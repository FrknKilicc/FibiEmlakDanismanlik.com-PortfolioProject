using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Neighborhood
    {
        [Key]
        public int NeighborhoodId { get; set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
    }
}
