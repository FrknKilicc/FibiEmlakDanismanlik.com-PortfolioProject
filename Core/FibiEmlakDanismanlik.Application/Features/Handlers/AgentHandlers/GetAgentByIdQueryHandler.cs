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
    public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, GetAgentByIdResult>
    {
        private readonly IRepository<Agent> _repository;

        public GetAgentByIdQueryHandler(IRepository<Agent> repository)
        {
            _repository = repository;
        }

        public async Task<GetAgentByIdResult> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetAgentByIdResult
            {
                AgentId = value.AgentId,
                AgentDescription= value.AgentDescription,
                AgentImgUrl = value.AgentImgUrl,
                AgentName = value.AgentName,
                AgentPhoneNumber = value.AgentPhoneNumber,
                AgentTitle = value.AgentTitle,
                Mail=value.Mail,
                Password = value.Password
            };
        }
    }
}
