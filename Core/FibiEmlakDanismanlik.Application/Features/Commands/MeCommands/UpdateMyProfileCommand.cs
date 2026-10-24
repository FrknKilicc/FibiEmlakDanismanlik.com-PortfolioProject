using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MeCommands
{
    /// <summary>
    /// Giriş yapmış çalışanın profil bilgilerini güncelleme komutu.
    /// </summary>
    public class UpdateMyProfileCommand : IRequest
    {
        public string AgentName { get; set; } = string.Empty;
        public string AgentPhoneNumber { get; set; } = string.Empty;
        public string? AgentTitle { get; set; }
        public string? AgentDescription { get; set; }
        public string? AgentImgUrl { get; set; }
    }
}
