using FibiEmlakDanismanlik.Application.Features.Queries.AuthorQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AuthorResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthorHandlers
{
    public class GetAuthorQueryHandler : IRequestHandler<GetAuthorQuery, List<GetAuthorResult>>
    {
        private readonly IRepository<Author> _repository;
        private readonly IBlogRepository _blogRepository;

        public GetAuthorQueryHandler(IRepository<Author> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task<List<GetAuthorResult>> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
        {
            var authors = await _repository.GetAllAsync();
            var blogCounts = await _blogRepository.GetBlogCountsByAuthorAsync();

            return authors
                .OrderBy(x => x.AuthorName)
                .Select(x => new GetAuthorResult
                {
                    AuthorId = x.AuthorId,
                    AuthorImgUrl = x.AuthorImgUrl,
                    AuthorName = x.AuthorName,
                    BlogCount = blogCounts.TryGetValue(x.AuthorId, out var count) ? count : 0
                })
                .ToList();
        }
    }
}
