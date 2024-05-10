using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MainBannerCommands
{
    public class RemoveMainBannerCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveMainBannerCommand(int id)
        {
            Id = id;
        }
    }
}
