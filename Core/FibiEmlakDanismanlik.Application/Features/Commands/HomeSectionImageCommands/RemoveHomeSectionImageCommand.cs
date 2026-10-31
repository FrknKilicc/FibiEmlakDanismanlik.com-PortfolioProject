using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands
{
    public class RemoveHomeSectionImageCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveHomeSectionImageCommand(int id)
        {
            Id = id;
        }
    }
}
