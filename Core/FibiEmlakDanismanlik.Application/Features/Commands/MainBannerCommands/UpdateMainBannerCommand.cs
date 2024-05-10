using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MainBannerCommands
{
    public class UpdateMainBannerCommand:IRequest
    {
        public int MainBannerId { get; set; }
        public string MainBannerUrl { get; set; }
        public string? MainBannerDesc { get; set; }
    }
}
