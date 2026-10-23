using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AgentValidators
{
    public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
    {
        public CreateAgentCommandValidator()
        {
            RuleFor(x => x.AgentName)
                .NotEmpty().WithMessage("Danışman adı zorunludur.")
                .MaximumLength(100).WithMessage("Danışman adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(256).WithMessage("E-posta adresi en fazla 256 karakter olabilir.");

            RuleFor(x => x.AgentPhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.");

            RuleFor(x => x.AgentDescription)
                .MaximumLength(500).WithMessage("Danışman açıklaması en fazla 500 karakter olabilir.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(AuthConstants.MinPasswordLength).WithMessage($"Şifre en az {AuthConstants.MinPasswordLength} karakter olmalıdır.")
                .MaximumLength(128).WithMessage("Şifre en fazla 128 karakter olabilir.")
                .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Geçerli bir personel rolü seçiniz.");
        }
    }
}
