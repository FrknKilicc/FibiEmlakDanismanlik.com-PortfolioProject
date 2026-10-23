using System.Linq;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingAlertValidators
{
    /// <summary>
    /// Haber talebi ilan eşleşme/bildirim işaretleme komutu doğrulayıcısı.
    /// </summary>
    public class MarkListingAlertMatchCommandValidator : AbstractValidator<MarkListingAlertMatchCommand>
    {
        public MarkListingAlertMatchCommandValidator()
        {
            RuleFor(x => x.AlertId)
                .GreaterThan(0).WithMessage("Geçerli bir talep kimliği belirtilmelidir.");

            RuleFor(x => x.ListingTypeId)
                .Must(id => ListingAlertConstants.ValidListingTypeIds.Contains(id))
                .WithMessage("Geçersiz ilan türü.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0).WithMessage("İlan kimliği 0'dan büyük olmalıdır.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Geçerli bir eşleşme durumu belirtilmelidir.");
        }
    }
}
