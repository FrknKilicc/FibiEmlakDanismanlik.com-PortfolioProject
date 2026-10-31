using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AuthValidators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.");
        }
    }
}
