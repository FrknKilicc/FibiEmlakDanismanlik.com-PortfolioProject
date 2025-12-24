using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class District
    {
        [Key]
        public int DistrictId { get; set; }
        [Required]
        [MaxLength  (100)]
        public string Name { get; set; }
        public int CityId { get; set; }
        [ForeignKey((nameof(CityId)))]
        public City City { get; set; }
        public ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
    }
}
