using FibiEmlakDanismanlik.Application.Features.Commands.MainCategoryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MainCategoryHandlers
{
    public class RemoveMainCategoryCommandHandler : IRequestHandler<RemoveMainCategoryCommand>
    {
        private readonly IRepository<MainCategory> _repository;

        public RemoveMainCategoryCommandHandler(IRepository<MainCategory> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveMainCategoryCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
