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
    public class RemoveAboutUsCommandHandler : IRequestHandler<RemoveAboutUsCommand>
    {
        private readonly IRepository<AboutUs> _repository;

        public RemoveAboutUsCommandHandler(IRepository<AboutUs> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveAboutUsCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
