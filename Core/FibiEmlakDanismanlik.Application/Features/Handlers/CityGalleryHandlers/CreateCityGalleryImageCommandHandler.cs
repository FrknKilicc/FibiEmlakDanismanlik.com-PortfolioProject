using FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CityGalleryHandlers
{
    public class CreateCityGalleryImageCommandHandler : IRequestHandler<CreateCityGalleryImageCommand, int>
    {
        private readonly IRepository<CityGalleryImage> _repository;

        public CreateCityGalleryImageCommandHandler(IRepository<CityGalleryImage> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateCityGalleryImageCommand request, CancellationToken cancellationToken)
        {
            var entity = new CityGalleryImage
            {
                CityId = request.CityId,
                ImageUrl = request.ImageUrl.Trim(),
                Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
            return entity.CityGalleryImageId;
        }
    }
}
