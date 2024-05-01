using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands
{
    public class UpdateAboutUsCommand:IRequest
    {
        public int AboutUsId { get; set; }
        public string AboutUsTitle { get; set; }
        public string AboutUsDesc { get; set; }
        public string AboutUsImgUrl { get; set; }
    }
}
