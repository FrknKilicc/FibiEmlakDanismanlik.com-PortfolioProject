using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands
{
    public class UpdateSocialMediaCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }
}
