using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingAlertValidators
{
    /// <summary>
    /// Haber talebi durumu güncelleme komutu doğrulayıcısı.
    /// </summary>
    public class ChangeListingAlertStatusCommandValidator : AbstractValidator<ChangeListingAlertStatusCommand>
    {
        public ChangeListingAlertStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir talep kimliği belirtilmelidir.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Geçerli bir talep durumu belirtilmelidir.");
        }
    }
}
