using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, GetLoginResult?>
    {
        private static readonly Lazy<string> DummyHash = new(() => BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()));

        private readonly IAgentRepository _agentRepository;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IAgentRepository agentRepository, IConfiguration configuration)
        {
            _agentRepository = agentRepository;
            _configuration = configuration;
        }

        public async Task<GetLoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Mail) || string.IsNullOrEmpty(request.Password))
                return null;

            var mail = request.Mail.Trim();
            var agent = await _agentRepository.GetByMailAsync(mail);
            if (agent == null || string.IsNullOrEmpty(agent.Password))
            {
                // Kayıtlı olmayan e-posta da aynı sürede yanıt alsın (e-posta tahmini zorlaşsın).
                BCrypt.Net.BCrypt.Verify(request.Password, DummyHash.Value);
                return null;
            }

            bool verified;
            try
            {
                verified = BCrypt.Net.BCrypt.Verify(request.Password, agent.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Kayıtlı değer geçerli bir BCrypt hash'i değil.
                verified = false;
            }

            return verified ? JwtTokenGenerator.GenerateToken(agent, _configuration) : null;
        }
    }
}
