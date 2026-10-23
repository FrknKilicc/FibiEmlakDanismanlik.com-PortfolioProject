using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AdminListingValidators
{
    /// <summary>
    /// İlan durumunu değiştirme komutu için FluentValidation kuralları.
    /// </summary>
    public class ChangeAdminListingStatusCommandValidator : AbstractValidator<ChangeAdminListingStatusCommand>
    {
        public ChangeAdminListingStatusCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .Must(id => id is 1 or 2 or 3 or 7 or 8 or 9)
                .WithMessage("Geçersiz ilan türü.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("İlan durumu belirtilmelidir.")
                .Must(status => AdminListingConstants.AllStatuses.Contains(status))
                .WithMessage("Geçersiz ilan durumu.");

            // "Satıldı" yalnızca satılık ilanlarda seçilebilir
            RuleFor(x => x.Status)
                .Must((cmd, status) => !string.Equals(status, PropertyStatuses.Satildi, System.StringComparison.OrdinalIgnoreCase) || !AdminListingConstants.IsRent(cmd.ListingTypeId))
                .WithMessage("Satıldı durumu yalnızca satılık ilanlar için geçerlidir.");

            // "Kiralandı" yalnızca kiralık ilanlarda seçilebilir
            RuleFor(x => x.Status)
                .Must((cmd, status) => !string.Equals(status, PropertyStatuses.Kiralandi, System.StringComparison.OrdinalIgnoreCase) || AdminListingConstants.IsRent(cmd.ListingTypeId))
                .WithMessage("Kiralandı durumu yalnızca kiralık ilanlar için geçerlidir.");
        }
    }
}
