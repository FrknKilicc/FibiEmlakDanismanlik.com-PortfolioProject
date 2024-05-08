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
    public class CreateMainCategoryCommandHandler : IRequestHandler<CreateMainCategoryCommand>
    {
        private readonly IRepository<MainCategory> _repository;

        public CreateMainCategoryCommandHandler(IRepository<MainCategory> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateMainCategoryCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new MainCategory
            {
                CategoryName = request.CategoryName,
            });
        }
    }
}
