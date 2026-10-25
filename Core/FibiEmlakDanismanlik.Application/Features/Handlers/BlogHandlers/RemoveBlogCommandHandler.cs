using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
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
    public class RemoveBlogCommandHandler : IRequestHandler<RemoveBlogCommand>
    {
        private readonly IRepository<Blog> _repository;
        private readonly IBlogRepository _blogRepository;

        public RemoveBlogCommandHandler(IRepository<Blog> repository, IBlogRepository blogRepository)
        {
            _repository = repository;
            _blogRepository = blogRepository;
        }

        public async Task Handle(RemoveBlogCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("Blog bulunamadı.");
            }

            // Etiket bağlarını kaldırır; başka yazıda kullanılmayan etiketler de silinir
            await _blogRepository.SetBlogTagsAsync(value.BlogId, Array.Empty<string>());

            await _repository.RemoveAsync(value);
        }
    }
}
