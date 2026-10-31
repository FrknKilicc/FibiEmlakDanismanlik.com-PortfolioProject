using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateHomeSectionImageCommandValidator : AbstractValidator<CreateHomeSectionImageCommand>
    {
        public CreateHomeSectionImageCommandValidator()
        {
            RuleFor(x => x.SectionKey)
                .NotEmpty().WithMessage("Geçersiz bölüm.")
                .Must(key => key == HomeSectionImageConstants.SectionPopularPlaces || key == HomeSectionImageConstants.SectionTestimonials)
                .WithMessage("Geçersiz bölüm.");

            RuleFor(x => x.Slot)
                .InclusiveBetween(HomeSectionImageConstants.MinSlot, HomeSectionImageConstants.MaxSlot)
                .WithMessage($"Yuva (slot) {HomeSectionImageConstants.MinSlot} ile {HomeSectionImageConstants.MaxSlot} arasında olmalıdır.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Görsel adresi zorunludur.")
                .MaximumLength(HomeSectionImageConstants.ImageUrlMaxLength)
                .WithMessage($"Görsel adresi en fazla {HomeSectionImageConstants.ImageUrlMaxLength} karakter olabilir.")
                .Must(ContentUrlRules.IsSafeImageUrl).WithMessage("Görsel adresi geçerli bir bağlantı veya yol olmalıdır.");

            RuleFor(x => x.AltText)
                .MaximumLength(HomeSectionImageConstants.AltTextMaxLength)
                .WithMessage($"Alternatif metin en fazla {HomeSectionImageConstants.AltTextMaxLength} karakter olabilir.");
        }
    }
}
