using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AdminListingValidators
{
    /// <summary>
    /// Yakın yer listesi kaydetme komutu için FluentValidation kuralları.
    /// </summary>
    public class SetAdminListingNearbyPlacesCommandValidator : AbstractValidator<SetAdminListingNearbyPlacesCommand>
    {
        public const int MaxPlaces = 40;

        public SetAdminListingNearbyPlacesCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .Must(id => id is 1 or 2 or 3 or 7 or 8 or 9)
                .WithMessage("Geçersiz ilan türü.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.Places)
                .NotNull().WithMessage("Yakın yer listesi gönderilmelidir.")
                .Must(p => p == null || p.Count <= MaxPlaces)
                .WithMessage($"En fazla {MaxPlaces} yakın yer girilebilir.");

            RuleForEach(x => x.Places).ChildRules(place =>
            {
                place.RuleFor(p => p.NearbyCategoryId)
                    .GreaterThan(0).WithMessage("Yakın yer kategorisi seçilmelidir.");

                place.RuleFor(p => p.PlaceName)
                    .Must(n => !string.IsNullOrWhiteSpace(n)).WithMessage("Yer adı boş olamaz.")
                    .Must(n => n == null || n.Trim().Length <= 120).WithMessage("Yer adı en fazla 120 karakter olabilir.");

                place.RuleFor(p => p.DistanceKm)
                    .InclusiveBetween(0m, 100m).WithMessage("Uzaklık 0 ile 100 km arasında olmalıdır.");
            });
        }
    }
}
