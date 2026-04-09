using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetTopBlogCategoriesQueryHandler : IRequestHandler<GetTopBlogCategoriesQuery, List<BlogCategoryCountResult>>
    {
        private readonly IBlogRepository _blogRepository;

        public GetTopBlogCategoriesQueryHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<List<BlogCategoryCountResult>> Handle(GetTopBlogCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _blogRepository.GetTopBlogCategories(request.Take);
        }
    }
}
