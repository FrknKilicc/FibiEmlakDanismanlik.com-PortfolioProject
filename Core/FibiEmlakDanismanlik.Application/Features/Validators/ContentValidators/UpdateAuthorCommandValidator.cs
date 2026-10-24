using FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
    {
        public UpdateAuthorCommandValidator()
        {
            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Geçerli bir yazar kimliği belirtilmelidir.");

            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage("Yazar adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Yazar adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.AuthorImgUrl)
                .MaximumLength(500).WithMessage("Yazar görsel adresi en fazla 500 karakter olabilir.")
                .Must(ContentUrlRules.IsSafeImageUrl).WithMessage("Yazar görsel adresi geçerli bir bağlantı veya yol olmalıdır.")
                .When(x => !string.IsNullOrWhiteSpace(x.AuthorImgUrl));
        }
    }
}
