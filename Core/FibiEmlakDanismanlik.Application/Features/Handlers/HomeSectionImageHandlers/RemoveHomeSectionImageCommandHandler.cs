using FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.HomeSectionImageHandlers
{
    public class RemoveHomeSectionImageCommandHandler : IRequestHandler<RemoveHomeSectionImageCommand>
    {
        private readonly IRepository<HomeSectionImage> _repository;

        public RemoveHomeSectionImageCommandHandler(IRepository<HomeSectionImage> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveHomeSectionImageCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("Görsel bulunamadı.");
            }

            await _repository.RemoveAsync(value);
        }
    }
}
