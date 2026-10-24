using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands
{
    public class RemoveCityGalleryImageCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveCityGalleryImageCommand(int id)
        {
            Id = id;
        }
    }
}
