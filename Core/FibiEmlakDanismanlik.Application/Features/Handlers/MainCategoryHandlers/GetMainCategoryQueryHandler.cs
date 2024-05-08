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
    public class GetMainCategoryQueryHandler : IRequestHandler<GetMainCategoryQuery, List<GetMainCategoryResult>>
    {
        private readonly IRepository<MainCategory> _repository;

        public GetMainCategoryQueryHandler(IRepository<MainCategory> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetMainCategoryResult>> Handle(GetMainCategoryQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x=>new GetMainCategoryResult
            {
                CategoryId = x.CategoryId,
                CategoryName=x.CategoryName,
            }).ToList();
        }
    }
}
