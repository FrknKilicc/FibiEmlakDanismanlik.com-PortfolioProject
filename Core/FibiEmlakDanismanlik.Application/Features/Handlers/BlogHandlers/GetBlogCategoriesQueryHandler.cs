using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
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
        private readonly IBlogRepository _blogRepository;

        public GetBlogCategoriesQueryHandler(IRepository<BlogCategory> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task<List<BlogCategoryItemResult>> Handle(GetBlogCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllAsync();
            var blogCounts = await _blogRepository.GetBlogCountsByCategoryAsync();

            return categories
                .OrderBy(x => x.CategoryName)
                .Select(x => new BlogCategoryItemResult
                {
                    BlogCategoryId = x.BlogCategoryId,
                    CategoryName = x.CategoryName ?? string.Empty,
                    BlogCount = blogCounts.TryGetValue(x.BlogCategoryId, out var count) ? count : 0
                })
                .ToList();
        }
    }
}
