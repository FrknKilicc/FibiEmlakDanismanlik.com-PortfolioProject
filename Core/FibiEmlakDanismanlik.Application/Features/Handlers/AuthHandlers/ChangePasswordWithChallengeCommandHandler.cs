using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    public class ChangePasswordWithChallengeCommandHandler : IRequestHandler<ChangePasswordWithChallengeCommand, GetLoginResult?>
    {
        private readonly IRepository<Agent> _agentRepository;
        private readonly ILoginFlowService _loginFlowService;
        private readonly IConfiguration _configuration;

        public ChangePasswordWithChallengeCommandHandler(
            IRepository<Agent> agentRepository,
            ILoginFlowService loginFlowService,
            IConfiguration configuration)
        {
            _agentRepository = agentRepository;
            _loginFlowService = loginFlowService;
            _configuration = configuration;
        }

        public async Task<GetLoginResult?> Handle(ChangePasswordWithChallengeCommand request, CancellationToken cancellationToken)
        {
            // Challenge token'ı 'pwd' amacıyla doğrula
            var agentId = ChallengeTokenHelper.Validate(
                request.ChallengeToken,
                AuthConstants.PurposePasswordChange,
                _configuration,
                out var tokenSecurityStamp);

            if (agentId == null)
            {
                await _loginFlowService.LogAsync(string.Empty, null, succeeded: false, failureReason: AuthConstants.FailureInvalidChallenge);
                return null;
            }

            var agent = await _agentRepository.GetByIdAsync(agentId.Value);
            if (agent == null || !agent.IsActive || !string.Equals(agent.SecurityStamp, tokenSecurityStamp, StringComparison.Ordinal))
            {
                await _loginFlowService.LogAsync(agent?.Mail ?? string.Empty, agent?.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidChallenge);
                return null;
            }

            // Yeni şifrenin mevcut şifreyle aynı olup olmadığını kontrol et
            bool isSamePassword = false;
            try
            {
                isSamePassword = !string.IsNullOrEmpty(agent.Password) && BCrypt.Net.BCrypt.Verify(request.NewPassword, agent.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                isSamePassword = false;
            }

            if (isSamePassword)
            {
                throw new ValidationException(
                    "Yeni şifre eskisiyle aynı olamaz.",
                    new[] { new ValidationFailure(nameof(request.NewPassword), "Yeni şifre eskisiyle aynı olamaz.") });
            }

            // Şifreyi güncelle ve güvenlik damgasını yenile
            agent.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            agent.MustChangePassword = false;
            agent.PasswordChangedAtUtc = DateTime.UtcNow;
            agent.SecurityStamp = Guid.NewGuid().ToString("N");

            await _agentRepository.UpdateAsync(agent);

            // Giriş akışının bir sonraki adımına devam et
            return await _loginFlowService.CompleteAsync(agent);
        }
    }
}
