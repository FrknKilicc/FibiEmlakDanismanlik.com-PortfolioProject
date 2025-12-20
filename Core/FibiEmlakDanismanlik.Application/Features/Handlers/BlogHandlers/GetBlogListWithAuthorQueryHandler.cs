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
    public class GetBlogListWithAuthorQueryHandler : IRequestHandler<GetBlogListWithAuthorQuery, List<GetBlogDetailWithAuthorResult>>
    {
        private readonly IBlogRepository blogRepository;

        public GetBlogListWithAuthorQueryHandler(IBlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }



        public async Task<List<GetBlogDetailWithAuthorResult>> Handle(GetBlogListWithAuthorQuery request, CancellationToken cancellationToken)
        {
            var result = await blogRepository.GetBlogListWithAuthor();

            return result.Select(x => new GetBlogDetailWithAuthorResult
            {
                AuthorId = x.AuthorId,
                AuthorName = x.AuthorName,
                AuthorImgUrl = x.AuthorImgUrl,
                BlogId = x.BlogId,
                BlogTitle = x.BlogTitle,
                BlogDescription = x.BlogDescription,
                BlogImgUrl = x.BlogImgUrl,
                TopNews = x.TopNews,
                CreatedDate = x.CreatedDate
            }).ToList();

        }
    }
}
