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
    public class CreateCustomerContactCommandHandler : IRequestHandler<CreateCustomerContactCommand>
    {
        private readonly IRepository<CustomerContact> _repository;

        public CreateCustomerContactCommandHandler(IRepository<CustomerContact> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateCustomerContactCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new CustomerContact
            {
                CustomerContactName = request.CustomerContactName,
                CustomerContactMail = request.CustomerContactMail,
                CustomerContactPhone = request.CustomerContactPhone,
                CustomerContactMessage = request.CustomerContactMessage,
                PropertyListingId = request.PropertyListingId,

            });
        }
    }
}
