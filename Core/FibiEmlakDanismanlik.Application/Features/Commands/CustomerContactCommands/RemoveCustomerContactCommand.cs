using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands
{
    public class RemoveCustomerContactCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveCustomerContactCommand(int id)
        {
            Id = id;
        }
    }
}
