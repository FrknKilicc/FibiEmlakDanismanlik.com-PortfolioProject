using FibiEmlakDanismanlik.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ListingType
    {
        [Key]
        public int ListingTypeId { get; set; }
        public string Name { get; set; }
        public UsageType UsageType { get; set; }
    }
}
