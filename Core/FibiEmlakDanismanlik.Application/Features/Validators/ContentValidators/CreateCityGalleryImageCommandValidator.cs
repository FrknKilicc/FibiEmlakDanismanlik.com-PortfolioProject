using FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateCityGalleryImageCommandValidator : AbstractValidator<CreateCityGalleryImageCommand>
    {
        public CreateCityGalleryImageCommandValidator()
        {
            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("İl seçiniz.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Görsel adresi zorunludur.")
                .MaximumLength(500).WithMessage("Görsel adresi en fazla 500 karakter olabilir.")
                .Must(ContentUrlRules.IsSafeImageUrl).WithMessage("Görsel adresi geçerli bir bağlantı veya yol olmalıdır.");

            RuleFor(x => x.Title)
                .MaximumLength(150).WithMessage("Başlık en fazla 150 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.DisplayOrder)
                .InclusiveBetween(0, 999).WithMessage("Sıralama 0 ile 999 arasında olmalıdır.");
        }
    }
}
