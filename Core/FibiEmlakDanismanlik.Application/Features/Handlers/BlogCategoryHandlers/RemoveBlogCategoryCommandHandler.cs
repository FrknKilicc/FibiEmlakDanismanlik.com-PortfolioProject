using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BlogInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogCategoryHandlers
{
    public class RemoveBlogCategoryCommandHandler : IRequestHandler<RemoveBlogCategoryCommand>
    {
        private readonly IRepository<BlogCategory> _repository;
        private readonly IBlogRepository _blogRepository;

        public RemoveBlogCategoryCommandHandler(IRepository<BlogCategory> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task Handle(RemoveBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("Kategori bulunamadı.");
            }

            var counts = await _blogRepository.GetBlogCountsByCategoryAsync();
            if (counts.TryGetValue(request.Id, out var count) && count > 0)
            {
                throw new BlogContentConflictException($"Bu kategoride {count} blog yazısı var. Önce yazıların kategorisini değiştirin.");
            }

            await _blogRepository.DetachDeletedBlogsFromCategoryAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
