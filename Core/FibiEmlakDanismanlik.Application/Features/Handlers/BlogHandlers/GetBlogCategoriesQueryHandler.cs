using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogCategoriesQueryHandler : IRequestHandler<GetBlogCategoriesQuery, List<BlogCategoryItemResult>>
    {
        private readonly IRepository<BlogCategory> _repository;

        public GetBlogCategoriesQueryHandler(IRepository<BlogCategory> repository)
        {
            _repository = repository;
        }

        public async Task<List<BlogCategoryItemResult>> Handle(GetBlogCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllAsync();
            return categories
                .OrderBy(x => x.CategoryName)
                .Select(x => new BlogCategoryItemResult
                {
                    BlogCategoryId = x.BlogCategoryId,
                    CategoryName = x.CategoryName ?? string.Empty
                })
                .ToList();
        }
    }
}
