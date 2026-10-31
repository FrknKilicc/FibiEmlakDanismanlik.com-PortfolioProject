using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogTagNamesQueryHandler : IRequestHandler<GetBlogTagNamesQuery, List<string>>
    {
        private readonly IBlogRepository _blogRepository;

        public GetBlogTagNamesQueryHandler(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<List<string>> Handle(GetBlogTagNamesQuery request, CancellationToken cancellationToken)
        {
            return await _blogRepository.GetAllTagNamesAsync();
        }
    }
}
