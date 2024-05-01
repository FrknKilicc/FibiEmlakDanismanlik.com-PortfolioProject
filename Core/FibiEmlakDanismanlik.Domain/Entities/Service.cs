using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceImgUrl { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceDesc { get; set; }
    }
}
