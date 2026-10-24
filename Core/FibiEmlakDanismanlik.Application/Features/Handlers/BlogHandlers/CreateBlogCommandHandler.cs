using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
using FibiEmlakDanismanlik.Application.Helpers;
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
    public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand>
    {
        private readonly IRepository<Blog> _repository;
        private readonly IBlogRepository _blogRepository;

        public CreateBlogCommandHandler(IRepository<Blog> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = new Blog
            {
                AuthorId = request.AuthorId,
                BlogCategoryId = request.BlogCategoryId,
                BlogDescription = request.BlogDescription?.Trim() ?? string.Empty,
                BlogImgUrl = request.BlogImgUrl?.Trim() ?? string.Empty,
                BlogTitle = request.BlogTitle?.Trim() ?? string.Empty,
                CreatedDate = request.CreatedDate,
                TopNews = request.TopNews
            };

            await _repository.CreateAsync(blog);

            if (request.Tags != null)
            {
                await _blogRepository.SetBlogTagsAsync(blog.BlogId, BlogTagNormalizer.Normalize(request.Tags));
            }
        }
    }
}
