using FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AgentResult;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers
{
    public class GetAgentQueryHandler : IRequestHandler<GetAgentQuery, List<GetAgentResult>>
    {
        private readonly IRepository<Agent> _repository;

        public GetAgentQueryHandler(IRepository<Agent> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAgentResult>> Handle(GetAgentQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x=> new GetAgentResult
            {
                AgentDescription = x.AgentDescription,
                AgentId = x.AgentId,
                AgentImgUrl = x.AgentImgUrl,
                AgentName = x.AgentName,
                AgentPhoneNumber = x.AgentPhoneNumber,
                AgentTitle = x.AgentTitle,
                Mail=x.Mail, 
                Password=x.Password,
            }).ToList();
        }
    }
}
