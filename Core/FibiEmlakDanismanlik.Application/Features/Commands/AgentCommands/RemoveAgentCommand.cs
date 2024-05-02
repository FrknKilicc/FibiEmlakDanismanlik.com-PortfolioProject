using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands
{
    public class RemoveAgentCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveAgentCommand(int id)
        {
            Id = id;
        }
    }
}
