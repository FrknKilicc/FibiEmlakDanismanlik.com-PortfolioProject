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
    public class CreateMainBannerCommandHandler : IRequestHandler<CreateMainBannerCommand>
    {
        private readonly IRepository<MainBanner> _repository;

        public CreateMainBannerCommandHandler(IRepository<MainBanner> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateMainBannerCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new MainBanner
            {
                MainBannerUrl = request.MainBannerUrl,
                MainBannerDesc = request.MainBannerDesc,
            });
        }
    }
}
