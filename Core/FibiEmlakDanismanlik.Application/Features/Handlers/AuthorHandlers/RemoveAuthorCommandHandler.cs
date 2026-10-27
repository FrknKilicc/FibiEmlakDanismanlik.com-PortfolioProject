using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands;
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
    public class RemoveAuthorCommandHandler : IRequestHandler<RemoveAuthorCommand>
    {
        private readonly IRepository<Author> _repository;
        private readonly IBlogRepository _blogRepository;

        public RemoveAuthorCommandHandler(IRepository<Author> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task Handle(RemoveAuthorCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("Yazar bulunamadı.");
            }

            var counts = await _blogRepository.GetBlogCountsByAuthorAsync();
            if (counts.TryGetValue(request.Id, out var count) && count > 0)
            {
                throw new BlogContentConflictException($"Bu yazara ait {count} blog yazısı var. Önce yazıları başka bir yazara aktarın veya silin.");
            }

            // Blogs.AuthorId FK'si CASCADE: yazarın yalnızca silinmiş (soft delete) yazıları varsa onlar da kalıcı silinir
            await _repository.RemoveAsync(value);
        }
    }
}
