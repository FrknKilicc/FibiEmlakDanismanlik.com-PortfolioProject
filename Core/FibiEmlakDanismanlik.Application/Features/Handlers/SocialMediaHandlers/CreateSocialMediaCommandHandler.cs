using FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.SocialMediaHandlers
{
    public class CreateSocialMediaCommandHandler : IRequestHandler<CreateSocialMediaCommand>
    {
        private readonly IRepository<SocialMedia> _repository;

        public CreateSocialMediaCommandHandler(IRepository<SocialMedia> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateSocialMediaCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new SocialMedia
            {
                Name = request.Name.Trim(),
                Url = request.Url.Trim(),
                Icon = request.Icon?.Trim(),
                Order = request.Order,
                IsActive = request.IsActive,
                CreatedDate = TurkeyClock.Now
            });
        }
    }
}
