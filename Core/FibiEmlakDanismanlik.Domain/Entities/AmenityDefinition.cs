using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class AmenityDefinition
    {
        [Key]
        public int AmenityId { get; set; }
        public string Name { get; set; }
        public string?  GroupName { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool isActive { get; set; } = true;

        public bool AppliesToHousing { get; set; } = true;
        public bool AppliesToLand { get; set; } = true;
        public bool AppliesToCommercial { get; set; } = true;

    }
}
