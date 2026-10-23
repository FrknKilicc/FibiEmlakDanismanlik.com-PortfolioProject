using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingImageValidators
{
    /// <summary>
    /// Fotoğraf sıralama ve kapak belirleme komutu için FluentValidation kuralları.
    /// </summary>
    public class ReorderListingImagesCommandValidator : AbstractValidator<ReorderListingImagesCommand>
    {
        public ReorderListingImagesCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan türü belirtilmelidir.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.CoverImageId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir kapak fotoğrafı seçilmelidir.");

            RuleFor(x => x.OrderedImageIds)
                .NotNull()
                .WithMessage("Sıralanacak fotoğraf listesi boş olamaz.")
                .Must(list => list != null && list.Count > 0)
                .WithMessage("Sıralanacak fotoğraf listesi boş olamaz.");
        }
    }
}
