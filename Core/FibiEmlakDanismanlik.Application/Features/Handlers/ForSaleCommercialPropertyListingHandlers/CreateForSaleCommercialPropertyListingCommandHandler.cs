using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class CreateForSaleCommercialPropertyListingCommandHandler : IRequestHandler<CreateForSaleCommercialPropertyListingCommands>
    {
        public Task Handle(CreateForSaleCommercialPropertyListingCommands request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
