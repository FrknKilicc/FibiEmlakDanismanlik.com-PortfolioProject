using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BlogCategoryHandlers
{
    public class UpdateBlogCategoryCommandHandler : IRequestHandler<UpdateBlogCategoryCommand>
    {
        private readonly IRepository<BlogCategory> _repository;

        public UpdateBlogCategoryCommandHandler(IRepository<BlogCategory> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.BlogCategoryId);
            if (value == null)
            {
                throw new KeyNotFoundException("Kategori bulunamadı.");
            }

            var trimmedName = request.CategoryName?.Trim() ?? string.Empty;
            var categories = await _repository.GetAllAsync();

            var trCulture = new CultureInfo("tr-TR");
            var duplicateExists = categories.Any(c =>
                c.BlogCategoryId != request.BlogCategoryId &&
                string.Compare(c.CategoryName, trimmedName, trCulture, CompareOptions.IgnoreCase) == 0);

            if (duplicateExists)
            {
                throw new BlogContentConflictException("Bu adla bir kategori zaten var.");
            }

            value.CategoryName = trimmedName;
            await _repository.UpdateAsync(value);
        }
    }
}
