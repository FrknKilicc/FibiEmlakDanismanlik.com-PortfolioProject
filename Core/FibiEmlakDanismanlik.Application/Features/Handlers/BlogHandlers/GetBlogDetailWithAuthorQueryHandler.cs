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
    public class GetBlogDetailWithAuthorQueryHandler : IRequestHandler<GetBlogDetailWithAuthorQuery, List<GetBlogDetailWithAuthorResult>>
    {
        private readonly IBlogRepository _blogrepository;
        public GetBlogDetailWithAuthorQueryHandler(IBlogRepository blogrepository)
        {
            _blogrepository = blogrepository;
        }

        public async Task<List<GetBlogDetailWithAuthorResult>> Handle(GetBlogDetailWithAuthorQuery request, CancellationToken cancellationToken)
        {
            var value = await _blogrepository.GetLast3BlogWithAuthor();
            return value.Select(x => new GetBlogDetailWithAuthorResult
            {
                BlogId = x.BlogId,
                AuthorId = x.AuthorId,
                BlogDescription = x.BlogDescription,
                BlogImgUrl = x.BlogImgUrl,
                BlogTitle = x.BlogTitle,
                AuthorImgUrl = x.AuthorImgUrl,
                AuthorName = x.AuthorName,
                CreatedDate = x.CreatedDate,
                TopNews = x.TopNews,

            }).ToList();
        }
    }
}
