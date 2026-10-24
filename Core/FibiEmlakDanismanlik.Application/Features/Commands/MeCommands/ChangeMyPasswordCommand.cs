using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MeCommands
{
    /// <summary>
    /// Giriş yapmış çalışanın kendi şifresini değiştirme komutu.
    /// </summary>
    public class ChangeMyPasswordCommand : IRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
