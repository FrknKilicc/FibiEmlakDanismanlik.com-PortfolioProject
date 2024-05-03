using FibiEmlakDanismanlik.Application.Features.Commands.ContactCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ContactHandlers
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand>
    {
        private readonly IRepository<Contact> _repository;

        public UpdateContactCommandHandler(IRepository<Contact> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.ContactId);
            value.PhoneNumber2 = request.PhoneNumber2;
            value.PhoneNumber1 = request.PhoneNumber1;
            value.EMail1 = request.EMail1;
            value.EMail2 = request.EMail2;
            value.OfficeAddress = request.OfficeAddress;
            await _repository.UpdateAsync(value);
        }
    }
}
