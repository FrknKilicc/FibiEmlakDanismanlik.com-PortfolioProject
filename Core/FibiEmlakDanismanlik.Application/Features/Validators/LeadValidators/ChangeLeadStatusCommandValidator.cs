using FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.LeadValidators
{
    /// <summary>
    /// Talep durumu güncelleme komutu doğrulayıcısı.
    /// </summary>
    public class ChangeLeadStatusCommandValidator : AbstractValidator<ChangeLeadStatusCommand>
    {
        public ChangeLeadStatusCommandValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Geçerli bir talep durumu belirtilmelidir.");

            RuleFor(x => x.Note)
                .MaximumLength(2000).WithMessage("Not açıklaması en fazla 2000 karakter olabilir.");
        }
    }
}
