using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands
{
    public class RemoveForSaleCommercialPropertyListingCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveForSaleCommercialPropertyListingCommand(int id)
        {
            Id = id;
        }
    }
}
