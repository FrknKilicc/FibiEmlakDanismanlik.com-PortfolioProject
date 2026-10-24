using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands
{
    public class RemoveSocialMediaCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveSocialMediaCommand(int id)
        {
            Id = id;
        }
    }
}
