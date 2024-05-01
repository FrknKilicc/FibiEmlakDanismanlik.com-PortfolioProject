using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands
{
    public class RemoveAboutUsCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveAboutUsCommand(int id)
        {
            Id = id;
        }
    }
}
