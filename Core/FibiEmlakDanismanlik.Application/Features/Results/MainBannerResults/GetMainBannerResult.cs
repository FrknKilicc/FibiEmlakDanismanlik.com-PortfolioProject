using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.MainBannerResults
{
    public class GetMainBannerResult
    {
        public int MainBannerId { get; set; }
        public string MainBannerUrl { get; set; }
        public string? MainBannerDesc { get; set; }
    }
}
