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
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand>
    {
        private readonly IRepository<Contact> _repository;

        public CreateContactCommandHandler(IRepository<Contact> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new Contact
            {
                EMail1 = request.EMail1,
                EMail2 = request.EMail2,
                OfficeAddress = request.OfficeAddress,
                PhoneNumber1 = request.PhoneNumber1,
                PhoneNumber2 = request.PhoneNumber2,
            });
        }
    }
}
