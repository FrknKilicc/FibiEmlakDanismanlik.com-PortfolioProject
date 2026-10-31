using FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateBlogCategoryCommandValidator : AbstractValidator<UpdateBlogCategoryCommand>
    {
        public UpdateBlogCategoryCommandValidator()
        {
            RuleFor(x => x.BlogCategoryId)
                .GreaterThan(0).WithMessage("Geçerli bir kategori kimliği belirtilmelidir.");

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        }
    }
}
