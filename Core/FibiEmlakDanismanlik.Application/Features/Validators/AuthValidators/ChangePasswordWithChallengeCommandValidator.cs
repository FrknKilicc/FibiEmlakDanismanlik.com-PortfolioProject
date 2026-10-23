using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AuthValidators
{
    /// <summary>
    /// Challenge ile şifre değiştirme komutu için doğrulama kuralları.
    /// </summary>
    public class ChangePasswordWithChallengeCommandValidator : AbstractValidator<ChangePasswordWithChallengeCommand>
    {
        public ChangePasswordWithChallengeCommandValidator()
        {
            RuleFor(x => x.ChallengeToken)
                .NotEmpty()
                .WithMessage("Doğrulama belirteci (challenge token) zorunludur.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Yeni şifre alanı zorunludur.")
                .MinimumLength(AuthConstants.MinPasswordLength)
                .WithMessage($"Yeni şifre en az {AuthConstants.MinPasswordLength} karakter olmalıdır.")
                .MaximumLength(128)
                .WithMessage("Yeni şifre en fazla 128 karakter olabilir.")
                .Matches(@"[A-Z]")
                .WithMessage("Yeni şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]")
                .WithMessage("Yeni şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]")
                .WithMessage("Yeni şifre en az bir rakam içermelidir.");
        }
    }
}
