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
    public class GetBlogSuggestionsQueryHandler : IRequestHandler<GetBlogSuggestionsQuery, List<BlogSuggestionResult>>
    {
        private readonly IBlogRepository _blogRepository;

        public GetBlogSuggestionsQueryHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<List<BlogSuggestionResult>> Handle(GetBlogSuggestionsQuery request, CancellationToken cancellationToken)
        {
            return await _blogRepository.SearchBlogSuggestion(request.Q, request.Take);
        }
    }
}
