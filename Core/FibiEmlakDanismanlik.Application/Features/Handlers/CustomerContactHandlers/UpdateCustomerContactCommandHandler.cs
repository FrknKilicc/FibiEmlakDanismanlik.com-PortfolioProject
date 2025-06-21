using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CustomerContactHandlers
{
    public class UpdateCustomerContactCommandHandler : IRequestHandler<UpdateCustomerContactCommand>
    {
        private readonly IRepository<CustomerContact> _repository;

        public UpdateCustomerContactCommandHandler(IRepository<CustomerContact> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCustomerContactCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.CustomerContactId);

            value.CustomerContactName = request.CustomerContactName;
            value.CustomerContactPhone = request.CustomerContactPhone;
            value.CustomerContactMessage = request.CustomerContactMessage;
            value.PropertyListingId = request.PropertyListingId;
        }
    }
}
