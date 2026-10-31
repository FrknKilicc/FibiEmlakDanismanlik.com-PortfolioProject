using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands
{
    /// <summary>
    /// Çok adımlı giriş akışında şifre değiştirme zorunluluğu durumunda challenge token ile şifre güncelleme komutu.
    /// </summary>
    public class ChangePasswordWithChallengeCommand : IRequest<GetLoginResult?>
    {
        public string ChallengeToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
