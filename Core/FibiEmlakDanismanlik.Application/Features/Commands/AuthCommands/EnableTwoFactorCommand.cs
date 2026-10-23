using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands
{
    /// <summary>
    /// İki adımlı doğrulamayı test koduyla doğrulayıp etkinleştiren komut.
    /// </summary>
    public class EnableTwoFactorCommand : IRequest<GetEnableTwoFactorResult?>
    {
        /// <summary>
        /// Giriş akışında verilen '2fa-setup' amaçlı challenge token.
        /// </summary>
        public string ChallengeToken { get; set; } = string.Empty;

        /// <summary>
        /// Authenticator uygulamasından alınan 6 haneli doğrulama kodu.
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}
