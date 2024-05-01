using FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AboutUsHandlers
{
    public class CreateAboutUsCommandHandler : IRequestHandler<CreateAboutUsCommand>
    {
        private readonly IRepository<AboutUs> _repository;

        public CreateAboutUsCommandHandler(IRepository<AboutUs> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateAboutUsCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new AboutUs
            {
                AboutUsDesc = request.AboutUsDesc,
                AboutUsImgUrl = request.AboutUsImgUrl,
                AboutUsTitle = request.AboutUsTitle,
            });
        }
    }
}
