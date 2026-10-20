using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AgentValidators
{
    public class UpdateAgentCommandValidator : AbstractValidator<UpdateAgentCommand>
    {
        public UpdateAgentCommandValidator()
        {
            RuleFor(x => x.AgentId)
                .GreaterThan(0).WithMessage("Geçerli bir danışman ID'si giriniz.");

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

            When(x => !string.IsNullOrEmpty(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.");
            });
        }
    }
}
