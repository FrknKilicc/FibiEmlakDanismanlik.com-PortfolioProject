using FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.SocialMediaHandlers
{
    public class UpdateSocialMediaCommandHandler : IRequestHandler<UpdateSocialMediaCommand>
    {
        private readonly IRepository<SocialMedia> _repository;

        public UpdateSocialMediaCommandHandler(IRepository<SocialMedia> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateSocialMediaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Sosyal medya kaydı bulunamadı.");
            }

            entity.Name = request.Name.Trim();
            entity.Url = request.Url.Trim();
            entity.Icon = request.Icon?.Trim();
            entity.Order = request.Order;
            entity.IsActive = request.IsActive;

            await _repository.UpdateAsync(entity);
        }
    }
}
