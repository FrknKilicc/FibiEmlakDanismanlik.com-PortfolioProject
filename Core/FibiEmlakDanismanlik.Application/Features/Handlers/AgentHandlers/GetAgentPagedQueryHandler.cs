using FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AgentResult;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers
{
    public class GetAgentPagedQueryHandler : IRequestHandler<GetAgentPagedQuery, GetAgentPagedResult>
    {
        private readonly IAgentRepository _repository;

        public GetAgentPagedQueryHandler(IAgentRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAgentPagedResult> Handle(GetAgentPagedQuery request, CancellationToken cancellationToken)
        {
            var pageSize = request.PageSize <= 0 ? 3 : request.PageSize;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 50) pageSize = 50;

            var page = request.Page < 1 ? 1 : request.Page;

            var total = await _repository.GetCountAsync();

            var totalPages = total == 0 ? 1 : (int)Math.Ceiling((double)total / pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var agents = await _repository.GetPagedAsync((page - 1) * pageSize, pageSize);

            var items = agents.Select(x => new GetAgentResult
            {
                AgentId = x.AgentId,
                AgentName = x.AgentName,
                AgentTitle = x.AgentTitle,
                AgentDescription = x.AgentDescription,
                AgentImgUrl = x.AgentImgUrl,
                Mail = x.Mail,
                AgentPhoneNumber = x.AgentPhoneNumber
            }).ToList();

            return new GetAgentPagedResult
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
