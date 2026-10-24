using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingImageValidators
{
    /// <summary>
    /// Görsel türü ve başlığı güncelleme komutu için FluentValidation kuralları.
    /// </summary>
    public class UpdateListingImageKindCommandValidator : AbstractValidator<UpdateListingImageKindCommand>
    {
        public UpdateListingImageKindCommandValidator()
        {
            RuleFor(x => x.ListingImageId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir görsel kimliği belirtilmelidir.");

            RuleFor(x => x.Kind)
                .InclusiveBetween(0, 3)
                .WithMessage("Geçersiz görsel türü.");

            RuleFor(x => x.Caption)
                .MaximumLength(ListingImageConstants.MaxCaptionLength)
                .WithMessage("Başlık en fazla 100 karakter olabilir.");
        }
    }
}
