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
    public class UpdateMainBannerCommandHandler : IRequestHandler<UpdateMainBannerCommand>
    {
        private readonly IRepository<MainBanner> _repository;

        public UpdateMainBannerCommandHandler(IRepository<MainBanner> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateMainBannerCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.MainBannerId);
            value.MainBannerDesc = request.MainBannerDesc;
            value.MainBannerUrl = request.MainBannerUrl;
            await _repository.UpdateAsync(value);
        }
    }
}
