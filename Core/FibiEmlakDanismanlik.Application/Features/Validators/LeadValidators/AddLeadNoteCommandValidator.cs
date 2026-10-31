using FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.LeadValidators
{
    /// <summary>
    /// Talep notu ekleme komutu doğrulayıcısı.
    /// </summary>
    public class AddLeadNoteCommandValidator : AbstractValidator<AddLeadNoteCommand>
    {
        public AddLeadNoteCommandValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Not metni boş olamaz.")
                .Length(1, 2000).WithMessage("Not metni 1 ile 2000 karakter arasında olmalıdır.");
        }
    }
}
