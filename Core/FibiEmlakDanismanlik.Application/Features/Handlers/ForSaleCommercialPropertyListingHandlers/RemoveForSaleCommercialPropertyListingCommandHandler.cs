using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class RemoveForSaleCommercialPropertyListingCommandHandler:IRequestHandler<RemoveForSaleCommercialPropertyListingCommand>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;

        public RemoveForSaleCommercialPropertyListingCommandHandler(IRepository<ForSaleCommercialPropertyListing> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveForSaleCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
          var value = await _repository.GetByIdAsync(request.Id);
          await _repository.RemoveAsync(value);
        }
    }
}
