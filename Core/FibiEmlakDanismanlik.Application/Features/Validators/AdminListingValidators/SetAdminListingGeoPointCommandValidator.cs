using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AdminListingValidators
{
    /// <summary>
    /// Harita pini ayarlama komutu için FluentValidation kuralları.
    /// </summary>
    public class SetAdminListingGeoPointCommandValidator : AbstractValidator<SetAdminListingGeoPointCommand>
    {
        public SetAdminListingGeoPointCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .Must(id => id is 1 or 2 or 3 or 7 or 8 or 9)
                .WithMessage("Geçersiz ilan türü.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(35m, 43m)
                .WithMessage("Enlem koordinatı Türkiye sınırları (35 - 43) dahilinde olmalıdır.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(25m, 45.5m)
                .WithMessage("Boylam koordinatı Türkiye sınırları (25 - 45.5) dahilinde olmalıdır.");
        }
    }
}
