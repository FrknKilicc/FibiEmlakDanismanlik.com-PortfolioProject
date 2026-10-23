using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    /// <summary>
    /// İki adımlı doğrulama kurulumunu başlatan komut işleyicisi.
    /// </summary>
    public class BeginTwoFactorSetupCommandHandler : IRequestHandler<BeginTwoFactorSetupCommand, GetTwoFactorSetupResult?>
    {
        private readonly IRepository<Agent> _agentRepository;
        private readonly ILoginFlowService _loginFlowService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IConfiguration _configuration;

        public BeginTwoFactorSetupCommandHandler(
            IRepository<Agent> agentRepository,
            ILoginFlowService loginFlowService,
            ITwoFactorService twoFactorService,
            IConfiguration configuration)
        {
            _agentRepository = agentRepository;
            _loginFlowService = loginFlowService;
            _twoFactorService = twoFactorService;
            _configuration = configuration;
        }

        public async Task<GetTwoFactorSetupResult?> Handle(BeginTwoFactorSetupCommand request, CancellationToken cancellationToken)
        {
            // Challenge token'ı '2fa-setup' amacıyla doğrula
            var agentId = ChallengeTokenHelper.Validate(
                request.ChallengeToken,
                AuthConstants.PurposeTwoFactorSetup,
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

            // Yeni TOTP gizli anahtarı üret ve şifreleyerek sakla
            var secret = _twoFactorService.GenerateSecret();
            agent.TwoFactorSecretProtected = _twoFactorService.Protect(secret);
            agent.TwoFactorEnabled = false;

            await _agentRepository.UpdateAsync(agent);

            // Gizli anahtarı kullanıcıya 4'lü bloklar halinde gösterilecek şekilde formatla
            var formattedSecret = string.Join(" ", Enumerable.Range(0, (secret.Length + 3) / 4)
                .Select(i => secret.Substring(i * 4, Math.Min(4, secret.Length - i * 4))));

            var otpAuthUri = _twoFactorService.BuildOtpAuthUri(agent.Mail ?? string.Empty, secret);

            return new GetTwoFactorSetupResult
            {
                Secret = formattedSecret,
                OtpAuthUri = otpAuthUri
            };
        }
    }
}
