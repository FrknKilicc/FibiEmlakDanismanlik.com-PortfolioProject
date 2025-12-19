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
    public class GetBlogDetailWithAuthorByIdQueryHandler : IRequestHandler<GetBlogDetailWithAuthorByIdQuery, GetBlogDetailWithAuthorResult>
    {
        private readonly IBlogRepository _blogRepository;

        public GetBlogDetailWithAuthorByIdQueryHandler(IBlogRepository blogRepository)
        {
            this._blogRepository = blogRepository;
        }

        public async Task<GetBlogDetailWithAuthorResult> Handle(GetBlogDetailWithAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            return await _blogRepository.GetBlogDetailWithAuthorById(request.BlogId);
        }
    }
}
