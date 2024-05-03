using FibiEmlakDanismanlik.Application.Features.Queries.ContactQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ContactResults;
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
    public class GetContactByIdHandler : IRequestHandler<GetContactByIdQuery, GetContactByIdResult>
    {
        private readonly IRepository<Contact> _repository;

        public GetContactByIdHandler(IRepository<Contact> repository)
        {
            _repository = repository;
        }

        public async Task<GetContactByIdResult> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetContactByIdResult
            {
                ContactId = value.ContactId,
                EMail1 = value.EMail1,
                EMail2 = value.EMail2,
                OfficeAddress = value.OfficeAddress,
                PhoneNumber1 = value.PhoneNumber1,
                PhoneNumber2 = value.PhoneNumber2,
            };
        }
    }
}
