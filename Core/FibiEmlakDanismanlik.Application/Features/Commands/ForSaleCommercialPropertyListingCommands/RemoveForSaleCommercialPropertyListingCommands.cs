using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands
{
    public class RemoveForSaleCommercialPropertyListingCommands:IRequest
    {
        public int Id { get; set; }

        public RemoveForSaleCommercialPropertyListingCommands(int id)
        {
            Id = id;
        }
    }
}
