using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands
{
    public class CreateCityGalleryImageCommand : IRequest<int>
    {
        public int CityId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
