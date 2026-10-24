using FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateFrequentlyAskedQuestionCommandValidator : AbstractValidator<UpdateFrequentlyAskedQuestionCommand>
    {
        public UpdateFrequentlyAskedQuestionCommandValidator()
        {
            RuleFor(x => x.FrequentlyAskedQuestionId)
                .GreaterThan(0).WithMessage("Geçerli bir soru kimliği belirtilmelidir.");

            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Soru boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Soru en fazla 500 karakter olabilir.");

            RuleFor(x => x.Answer)
                .MaximumLength(4000).WithMessage("Cevap en fazla 4000 karakter olabilir.");
        }
    }
}
