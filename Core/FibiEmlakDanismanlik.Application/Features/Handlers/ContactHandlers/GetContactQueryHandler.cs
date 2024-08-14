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
    public class GetContactQueryHandler : IRequestHandler<GetContactQuery, List<GetContactResult>>
    {
        private IRepository<Contact> _repository;

        public GetContactQueryHandler(IRepository<Contact> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetContactResult>> Handle(GetContactQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x => new GetContactResult
            {
                ContactId = x.ContactId,
                EMail1 = x.EMail1,
                EMail2 = x.EMail2,
                OfficeAddress = x.OfficeAddress,
                PhoneNumber1 = x.PhoneNumber1,
                PhoneNumber2 = x.PhoneNumber2,
            }).ToList();
        }
    }
}
