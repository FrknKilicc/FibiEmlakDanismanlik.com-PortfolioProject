using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AuthValidators
{
    public class BeginTwoFactorSetupCommandValidator : AbstractValidator<BeginTwoFactorSetupCommand>
    {
        public BeginTwoFactorSetupCommandValidator()
        {
            RuleFor(x => x.ChallengeToken)
                .NotEmpty().WithMessage("Doğrulama belirteci (ChallengeToken) zorunludur.");
        }
    }
}
