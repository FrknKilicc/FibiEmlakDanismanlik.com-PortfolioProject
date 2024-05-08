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
    public class UpdateMainCategoryCommandHandler : IRequestHandler<UpdateMainCategoryCommand>
    {
        private readonly IRepository<MainCategory> _repository;

        public UpdateMainCategoryCommandHandler(IRepository<MainCategory> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateMainCategoryCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.CategoryId);
            value.CategoryName = request.CategoryName;
            await _repository.UpdateAsync(value);
        }
    }
}
