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
    public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand>
    {
        private readonly IRepository<Blog> _repository;
        private readonly IBlogRepository _blogRepository;

        public UpdateBlogCommandHandler(IRepository<Blog> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.BlogId);
            if (value == null)
            {
                throw new KeyNotFoundException("Blog bulunamadı.");
            }

            value.BlogTitle = request.BlogTitle?.Trim() ?? string.Empty;
            value.BlogDescription = request.BlogDescription?.Trim() ?? string.Empty;
            value.BlogImgUrl = request.BlogImgUrl?.Trim() ?? string.Empty;
            value.CreatedDate = request.CreatedDate;
            value.AuthorId = request.AuthorId;
            value.BlogCategoryId = request.BlogCategoryId;
            value.TopNews = request.TopNews;

            await _repository.UpdateAsync(value);

            if (request.Tags != null)
            {
                await _blogRepository.SetBlogTagsAsync(value.BlogId, BlogTagNormalizer.Normalize(request.Tags));
            }
        }
    }
}
