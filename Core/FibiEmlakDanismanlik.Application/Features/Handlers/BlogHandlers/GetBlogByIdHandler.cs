using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogByIdHandler : IRequestHandler<GetBlogByIdQuery, GetBlogByIdResult>
    {
        private readonly IRepository<Blog> _repository;
        private readonly IBlogRepository _blogRepository;

        public GetBlogByIdHandler(IRepository<Blog> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task<GetBlogByIdResult> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                return null!;
            }

            var tags = await _blogRepository.GetTagNamesAsync(value.BlogId);

            return new GetBlogByIdResult
            {
                AuthorId = value.AuthorId,
                BlogCategoryId = value.BlogCategoryId,
                BlogDescription = value.BlogDescription ?? string.Empty,
                BlogId = value.BlogId,
                BlogImgUrl = value.BlogImgUrl ?? string.Empty,
                BlogTitle = value.BlogTitle ?? string.Empty,
                CreatedDate = value.CreatedDate,
                TopNews = value.TopNews,
                Tags = tags
            };
        }
    }
}
