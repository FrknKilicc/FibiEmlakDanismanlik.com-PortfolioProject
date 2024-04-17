using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class MainBanner
    {
        public int MainBannerId { get; set; }
        [Required]
        public string MainBannerUrl { get; set; }
        public string? MainBannerDesc { get; set; }
    }
}
