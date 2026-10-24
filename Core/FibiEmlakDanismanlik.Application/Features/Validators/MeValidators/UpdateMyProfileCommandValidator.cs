using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.MeValidators
{
    /// <summary>
    /// Profil güncelleme komutuna ilişkin doğrulama kuralları.
    /// </summary>
    public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
    {
        public UpdateMyProfileCommandValidator()
        {
            RuleFor(x => x.AgentName)
                .NotEmpty()
                .WithMessage("Ad soyad alanı zorunludur.")
                .MaximumLength(100)
                .WithMessage("Ad soyad en fazla 100 karakter olabilir.");

            RuleFor(x => x.AgentPhoneNumber)
                .NotEmpty()
                .WithMessage("Telefon numarası zorunludur.")
                .MaximumLength(20)
                .WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[0-9\s+()\-]+$")
                .WithMessage("Geçerli bir telefon numarası giriniz.");

            RuleFor(x => x.AgentTitle)
                .MaximumLength(100)
                .WithMessage("Unvan en fazla 100 karakter olabilir.");

            RuleFor(x => x.AgentDescription)
                .MaximumLength(2000)
                .WithMessage("Açıklama en fazla 2000 karakter olabilir.");

            When(x => !string.IsNullOrWhiteSpace(x.AgentImgUrl), () =>
            {
                RuleFor(x => x.AgentImgUrl)
                    .Must(ContentUrlRules.IsSafeImageUrl)
                    .WithMessage("Geçersiz veya güvensiz görsel adresi.");
            });
        }
    }
}
