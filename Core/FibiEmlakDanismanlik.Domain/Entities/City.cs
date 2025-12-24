using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public int? PlaceCode { get; set; }
        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
