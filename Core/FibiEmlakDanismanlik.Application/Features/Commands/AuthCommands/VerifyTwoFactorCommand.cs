using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands
{
    /// <summary>
    /// İki adımlı doğrulama aşamasında TOTP kodu veya kurtarma kodu ile oturum açmayı sağlayan komut.
    /// </summary>
    public class VerifyTwoFactorCommand : IRequest<GetLoginResult?>
    {
        /// <summary>
        /// Giriş akışında verilen '2fa' amaçlı challenge token.
        /// </summary>
        public string ChallengeToken { get; set; } = string.Empty;

        /// <summary>
        /// Authenticator uygulamasından alınan 6 haneli TOTP kodu (opsiyonel).
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Tek kullanımlık kurtarma kodu (opsiyonel).
        /// </summary>
        public string? RecoveryCode { get; set; }
    }
}
