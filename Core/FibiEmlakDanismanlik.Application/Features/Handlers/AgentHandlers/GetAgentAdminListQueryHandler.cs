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
    public class GetAgentAdminListQueryHandler : IRequestHandler<GetAgentAdminListQuery, List<GetAgentAdminListResult>>
    {
        private readonly IAgentRepository _agentRepository;

        public GetAgentAdminListQueryHandler(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        public async Task<List<GetAgentAdminListResult>> Handle(GetAgentAdminListQuery request, CancellationToken cancellationToken)
        {
            var agents = await _agentRepository.GetAllForAdminAsync();
            var now = DateTime.UtcNow;

            return agents.Select(x => new GetAgentAdminListResult
            {
                AgentId = x.AgentId,
                AgentName = x.AgentName,
                AgentTitle = x.AgentTitle,
                Mail = x.Mail,
                AgentPhoneNumber = x.AgentPhoneNumber,
                AgentImgUrl = x.AgentImgUrl,
                Role = x.Role.ToString(),
                IsActive = x.IsActive,
                ShowOnWebsite = x.ShowOnWebsite,
                CanViewAllListings = x.CanViewAllListings,
                TwoFactorEnabled = x.TwoFactorEnabled,
                MustChangePassword = x.MustChangePassword,
                IsLockedOut = x.LockoutEndUtc.HasValue && x.LockoutEndUtc.Value > now,
                LastLoginAtUtc = x.LastLoginAtUtc
            }).ToList();
        }
    }
}
