using FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateFrequentlyAskedQuestionCommandValidator : AbstractValidator<CreateFrequentlyAskedQuestionCommand>
    {
        public CreateFrequentlyAskedQuestionCommandValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Soru boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Soru en fazla 500 karakter olabilir.");

            RuleFor(x => x.Answer)
                .MaximumLength(4000).WithMessage("Cevap en fazla 4000 karakter olabilir.");
        }
    }
}
