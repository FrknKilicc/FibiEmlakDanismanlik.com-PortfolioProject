using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands
{
    /// <summary>
    /// İki adımlı doğrulama kurulumunu başlatan komut.
    /// </summary>
    public class BeginTwoFactorSetupCommand : IRequest<GetTwoFactorSetupResult?>
    {
        /// <summary>
        /// Giriş akışında verilen '2fa-setup' amaçlı challenge token.
        /// </summary>
        public string ChallengeToken { get; set; } = string.Empty;
    }
}
