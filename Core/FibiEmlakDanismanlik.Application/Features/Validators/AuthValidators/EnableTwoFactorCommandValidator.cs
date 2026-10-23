using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AuthValidators
{
    public class EnableTwoFactorCommandValidator : AbstractValidator<EnableTwoFactorCommand>
    {
        public EnableTwoFactorCommandValidator()
        {
            RuleFor(x => x.ChallengeToken)
                .NotEmpty().WithMessage("Doğrulama belirteci (ChallengeToken) zorunludur.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Doğrulama kodu zorunludur.")
                .Length(6).WithMessage("Doğrulama kodu 6 haneli olmalıdır.")
                .Matches(@"^\d{6}$").WithMessage("Doğrulama kodu 6 rakamdan oluşmalıdır.");
        }
    }
}
