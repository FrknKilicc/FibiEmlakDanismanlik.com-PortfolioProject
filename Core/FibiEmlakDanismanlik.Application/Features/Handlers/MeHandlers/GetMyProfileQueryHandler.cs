using FibiEmlakDanismanlik.Application.Features.Queries.MeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MeResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Giriş yapmış çalışanın profil bilgilerini ve iki adımlı doğrulama durumunu getiren işleyici.
    /// </summary>
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, GetMyProfileResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAgentRepository _agentCustomRepository;

        public GetMyProfileQueryHandler(
            ICurrentUserService currentUserService,
            IRepository<Agent> agentRepository,
            IAgentRepository agentCustomRepository)
        {
            _currentUserService = currentUserService;
            _agentRepository = agentRepository;
            _agentCustomRepository = agentCustomRepository;
        }

        public async Task<GetMyProfileResult> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var agentId = _currentUserService.AgentId;
            if (agentId == null)
            {
                throw new UnauthorizedAccessException("Oturum açmış kullanıcı bulunamadı.");
            }

            var agent = await _agentRepository.GetByIdAsync(agentId.Value);
            if (agent == null)
            {
                throw new KeyNotFoundException("Kullanıcı hesabı bulunamadı.");
            }

            var unusedCodes = await _agentCustomRepository.GetUnusedRecoveryCodesAsync(agentId.Value);

            return new GetMyProfileResult
            {
                AgentId = agent.AgentId,
                AgentName = agent.AgentName ?? string.Empty,
                Mail = agent.Mail ?? string.Empty,
                AgentPhoneNumber = agent.AgentPhoneNumber ?? string.Empty,
                AgentTitle = agent.AgentTitle,
                AgentDescription = agent.AgentDescription,
                AgentImgUrl = agent.AgentImgUrl,
                Role = agent.Role.ToString(),
                TwoFactorEnabled = agent.TwoFactorEnabled,
                UnusedRecoveryCodeCount = unusedCodes.Count,
                LastLoginAtUtc = agent.LastLoginAtUtc,
                PasswordChangedAtUtc = agent.PasswordChangedAtUtc
            };
        }
    }
}
