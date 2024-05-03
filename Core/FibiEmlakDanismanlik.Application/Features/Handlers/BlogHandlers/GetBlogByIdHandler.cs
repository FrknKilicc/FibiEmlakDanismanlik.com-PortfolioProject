using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogByIdHandler : IRequestHandler<GetBlogByIdQuery, GetBlogByIdResult>
    {
        private readonly IRepository<Blog> _repository;

        public GetBlogByIdHandler(IRepository<Blog> repository)
        {
            _repository = repository;
        }

        public async Task<GetBlogByIdResult> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetBlogByIdResult
            {
                AuthorId = value.AuthorId,
                BlogDescription = value.BlogDescription,
                BlogId = value.BlogId,
                BlogImgUrl = value.BlogImgUrl,
                BlogTitle = value.BlogTitle,
                CreatedDate = value.CreatedDate,
                TopNews = value.TopNews,
            };
        }
    }
}
