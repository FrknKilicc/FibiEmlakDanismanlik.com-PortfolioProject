using FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateBlogCategoryCommandValidator : AbstractValidator<CreateBlogCategoryCommand>
    {
        public CreateBlogCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        }
    }
}
