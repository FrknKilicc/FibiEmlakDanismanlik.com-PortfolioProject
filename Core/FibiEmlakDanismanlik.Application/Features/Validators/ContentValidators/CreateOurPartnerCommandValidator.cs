using FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateOurPartnerCommandValidator : AbstractValidator<CreateOurPartnerCommand>
    {
        public CreateOurPartnerCommandValidator()
        {
            RuleFor(x => x.PartnerName)
                .NotEmpty().WithMessage("Partner adı zorunludur.")
                .MaximumLength(100).WithMessage("Partner adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.PartnerLogoUrl)
                .NotEmpty().WithMessage("Logo adresi zorunludur.")
                .MaximumLength(500).WithMessage("Logo adresi en fazla 500 karakter olabilir.")
                .Must(ContentUrlRules.IsSafeImageUrl).WithMessage("Geçerli bir logo adresi girin.");

            RuleFor(x => x.DisplayOrder)
                .InclusiveBetween(0, 999).WithMessage("Sıralama 0 ile 999 arasında olmalıdır.");
        }
    }
}
