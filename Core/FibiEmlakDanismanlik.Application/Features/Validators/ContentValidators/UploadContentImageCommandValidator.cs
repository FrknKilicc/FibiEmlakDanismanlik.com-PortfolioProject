using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    /// <summary>
    /// İçerik görseli yükleme komutu için doğrulama kuralları.
    /// </summary>
    public class UploadContentImageCommandValidator : AbstractValidator<UploadContentImageCommand>
    {
        public UploadContentImageCommandValidator()
        {
            RuleFor(x => x.Folder)
                .Must(ContentImageConstants.IsAllowedFolder)
                .WithMessage("Geçersiz görsel klasörü.");

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("Yüklenecek dosya seçilmedi.");

            RuleFor(x => x.File)
                .Must(f => f!.Length > 0 && f.Length <= ListingImageConstants.MaxUploadBytes)
                .WithMessage("Dosya boyutu en fazla 15 MB olabilir.")
                .When(x => x.File != null);
        }
    }
}
