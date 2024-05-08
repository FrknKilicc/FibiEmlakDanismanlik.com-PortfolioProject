using FibiEmlakDanismanlik.Application.Features.Queries.MainCategoryQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MainCategoryResults;
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
    public class GetMainCategoryQueryByIdHandler : IRequestHandler<GetMainCategoryByIdQuery, GetMainCategoryByIdResult>
    {
        private readonly IRepository<MainCategory> _repository;

        public GetMainCategoryQueryByIdHandler(IRepository<MainCategory> repository)
        {
            _repository = repository;
        }

        public async Task<GetMainCategoryByIdResult> Handle(GetMainCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetMainCategoryByIdResult
            {
                CategoryId = value.CategoryId,
                CategoryName = value.CategoryName,
            };
        }
    }
}
