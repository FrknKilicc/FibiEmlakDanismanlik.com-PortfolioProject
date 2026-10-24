using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands
{
    public class CreateSocialMediaCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
