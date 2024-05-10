using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.MainBannerDtos
{
    public class ResultMainBannerDto
    {
        public int MainBannerId { get; set; }
        public string MainBannerUrl { get; set; }
        public string? MainBannerDesc { get; set; }
    }
}
