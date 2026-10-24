using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands
{
    public class UpdateCityGalleryImageCommand : IRequest
    {
        public int CityGalleryImageId { get; set; }
        public int CityId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
