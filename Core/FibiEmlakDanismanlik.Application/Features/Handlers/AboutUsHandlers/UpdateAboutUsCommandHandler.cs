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
    public class UpdateAboutUsCommandHandler : IRequestHandler<UpdateAboutUsCommand>
    {
        private readonly IRepository<AboutUs> _repository;

        public UpdateAboutUsCommandHandler(IRepository<AboutUs> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateAboutUsCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.AboutUsId);
            value.AboutUsId = request.AboutUsId;
            value.AboutUsImgUrl = request.AboutUsImgUrl;
            value.AboutUsDesc = request.AboutUsDesc;
            value.AboutUsTitle = request.AboutUsTitle;
            await _repository.UpdateAsync(value);
        }
    }
}
