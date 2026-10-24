using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogCategoryHandlers
{
    public class CreateBlogCategoryCommandHandler : IRequestHandler<CreateBlogCategoryCommand>
    {
        private readonly IRepository<BlogCategory> _repository;

        public CreateBlogCategoryCommandHandler(IRepository<BlogCategory> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            var trimmedName = request.CategoryName?.Trim() ?? string.Empty;
            var categories = await _repository.GetAllAsync();

            var trCulture = new CultureInfo("tr-TR");
            var duplicateExists = categories.Any(c =>
                string.Compare(c.CategoryName, trimmedName, trCulture, CompareOptions.IgnoreCase) == 0);

            if (duplicateExists)
            {
                throw new BlogContentConflictException("Bu adla bir kategori zaten var.");
            }

            await _repository.CreateAsync(new BlogCategory
            {
                CategoryName = trimmedName
            });
        }
    }
}
