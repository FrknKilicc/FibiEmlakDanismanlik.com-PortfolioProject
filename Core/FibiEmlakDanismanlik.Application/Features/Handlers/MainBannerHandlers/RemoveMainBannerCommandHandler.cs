using FibiEmlakDanismanlik.Application.Features.Commands.MainBannerCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MainBannerHandlers
{
    public class RemoveMainBannerCommandHandler : IRequestHandler<RemoveMainBannerCommand>
    {
        private readonly IRepository<MainBanner> _repository;

        public RemoveMainBannerCommandHandler(IRepository<MainBanner> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveMainBannerCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
