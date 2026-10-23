using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, GetLoginResult?>
    {
        private static readonly Lazy<string> DummyHash = new(() => BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()));

        private readonly IAgentRepository _agentRepository;
        private readonly ICaptchaVerifier _captchaVerifier;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILoginFlowService _loginFlowService;

        public LoginCommandHandler(
            IAgentRepository agentRepository,
            ICaptchaVerifier captchaVerifier,
            ICurrentUserService currentUserService,
            ILoginFlowService loginFlowService)
        {
            _agentRepository = agentRepository;
            _captchaVerifier = captchaVerifier;
            _currentUserService = currentUserService;
            _loginFlowService = loginFlowService;
        }

        public async Task<GetLoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var attemptedMail = request.Mail?.Trim() ?? string.Empty;

            // 1. Bot doğrulaması (Captcha)
            var isCaptchaValid = await _captchaVerifier.VerifyAsync(request.CaptchaToken, _currentUserService.IpAddress, cancellationToken);
            if (!isCaptchaValid)
            {
                await _loginFlowService.LogAsync(attemptedMail, null, succeeded: false, failureReason: AuthConstants.FailureCaptcha);
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Mail) || string.IsNullOrEmpty(request.Password))
                return null;

            // 2. Sabit süreli kontrol; kayıt yoksa log ve null
            var agent = await _agentRepository.GetByMailAsync(attemptedMail);
            if (agent == null || string.IsNullOrEmpty(agent.Password))
            {
                // Kayıtlı olmayan e-posta da aynı sürede yanıt alsın (e-posta tahmini zorlaşsın).
                BCrypt.Net.BCrypt.Verify(request.Password, DummyHash.Value);
                await _loginFlowService.LogAsync(attemptedMail, null, succeeded: false, failureReason: AuthConstants.FailureInvalidCredentials);
                return null;
            }

            // 3. Hesap kilitliyse: şifreyi yine doğrula (süre farkı olmasın) ama sonucu yok say; log ve null
            if (_loginFlowService.IsLockedOut(agent))
            {
                try
                {
                    BCrypt.Net.BCrypt.Verify(request.Password, agent.Password);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                }

                await _loginFlowService.LogAsync(attemptedMail, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureLockedOut);
                return null;
            }

            // 4. Şifre doğrulaması; yanlışsa RegisterFailureAsync + log + null
            bool verified;
            try
            {
                verified = BCrypt.Net.BCrypt.Verify(request.Password, agent.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                verified = false;
            }

            if (!verified)
            {
                await _loginFlowService.RegisterFailureAsync(agent);
                await _loginFlowService.LogAsync(attemptedMail, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidCredentials);
                return null;
            }

            // 5. IsActive false ise log + null
            if (!agent.IsActive)
            {
                await _loginFlowService.LogAsync(attemptedMail, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureInactive);
                return null;
            }

            // 6. Başarılı ise sıradaki adımı belirle ve sonucu dön
            return await _loginFlowService.CompleteAsync(agent);
        }
    }
}
