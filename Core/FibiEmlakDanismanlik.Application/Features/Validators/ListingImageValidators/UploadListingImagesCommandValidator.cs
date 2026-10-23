using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingImageValidators
{
    /// <summary>
    /// İlan görsel yükleme komutu için FluentValidation kuralları.
    /// </summary>
    public class UploadListingImagesCommandValidator : AbstractValidator<UploadListingImagesCommand>
    {
        public UploadListingImagesCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan türü belirtilmelidir.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.Files)
                .NotNull()
                .WithMessage("Yüklenecek dosya seçilmedi.")
                .Must(files => files != null && files.Count >= 1 && files.Count <= ListingImageConstants.MaxUploadFilesPerRequest)
                .WithMessage($"Tek bir istekte en az 1, en fazla {ListingImageConstants.MaxUploadFilesPerRequest} dosya yüklenebilir.");

            RuleForEach(x => x.Files)
                .Must(file => file != null && file.Length > 0 && file.Length <= ListingImageConstants.MaxUploadBytes)
                .WithMessage("Her dosya en fazla 15 MB olabilir ve boş olamaz.");
        }
    }
}
