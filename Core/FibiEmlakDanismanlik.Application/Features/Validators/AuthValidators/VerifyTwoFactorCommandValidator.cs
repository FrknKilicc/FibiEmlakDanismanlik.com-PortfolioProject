using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AuthValidators
{
    public class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
    {
        public VerifyTwoFactorCommandValidator()
        {
            RuleFor(x => x.ChallengeToken)
                .NotEmpty().WithMessage("Doğrulama belirteci (ChallengeToken) zorunludur.");

            RuleFor(x => x)
                .Must(x => (!string.IsNullOrWhiteSpace(x.Code) && string.IsNullOrWhiteSpace(x.RecoveryCode)) ||
                           (string.IsNullOrWhiteSpace(x.Code) && !string.IsNullOrWhiteSpace(x.RecoveryCode)))
                .WithMessage("Doğrulama kodu veya kurtarma kodundan tam biri girilmelidir.");
        }
    }
}
