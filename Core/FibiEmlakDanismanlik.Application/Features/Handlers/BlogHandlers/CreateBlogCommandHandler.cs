using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public CreateBlogCommandHandler(IRepository<Blog> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new Blog
            {
                AuthorId = request.AuthorId,
                BlogCategoryId = request.BlogCategoryId,
                BlogDescription = request.BlogDescription?.Trim() ?? string.Empty,
                BlogImgUrl = request.BlogImgUrl?.Trim() ?? string.Empty,
                BlogTitle = request.BlogTitle?.Trim() ?? string.Empty,
                CreatedDate = request.CreatedDate,
                TopNews = request.TopNews
            });
        }
    }
}
