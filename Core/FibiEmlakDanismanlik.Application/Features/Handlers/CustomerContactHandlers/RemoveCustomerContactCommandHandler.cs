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
    public class RemoveCustomerContactCommandHandler : IRequestHandler<RemoveCustomerContactCommand>
    {
        private readonly IRepository<CustomerContact> _repository;

        public RemoveCustomerContactCommandHandler(IRepository<CustomerContact> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveCustomerContactCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
