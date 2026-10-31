using FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CityGalleryHandlers
{
    public class RemoveCityGalleryImageCommandHandler : IRequestHandler<RemoveCityGalleryImageCommand>
    {
        private readonly IRepository<CityGalleryImage> _repository;

        public RemoveCityGalleryImageCommandHandler(IRepository<CityGalleryImage> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveCityGalleryImageCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Şehir görseli bulunamadı.");
            }

            await _repository.RemoveAsync(entity);
        }
    }
}
