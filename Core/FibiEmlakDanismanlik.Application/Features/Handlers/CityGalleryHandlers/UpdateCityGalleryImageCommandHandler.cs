using FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CityGalleryHandlers
{
    public class UpdateCityGalleryImageCommandHandler : IRequestHandler<UpdateCityGalleryImageCommand>
    {
        private readonly IRepository<CityGalleryImage> _repository;

        public UpdateCityGalleryImageCommandHandler(IRepository<CityGalleryImage> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCityGalleryImageCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.CityGalleryImageId);
            if (entity == null)
            {
                throw new KeyNotFoundException("Şehir görseli bulunamadı.");
            }

            entity.CityId = request.CityId;
            entity.ImageUrl = request.ImageUrl.Trim();
            entity.Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim();
            entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            entity.DisplayOrder = request.DisplayOrder;
            entity.IsActive = request.IsActive;

            await _repository.UpdateAsync(entity);
        }
    }
}
