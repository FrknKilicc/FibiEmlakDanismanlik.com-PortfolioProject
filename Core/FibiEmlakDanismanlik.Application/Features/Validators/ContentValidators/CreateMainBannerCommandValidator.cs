using FibiEmlakDanismanlik.Application.Features.Commands.MainBannerCommands;
using FluentValidation;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateMainBannerCommandValidator : AbstractValidator<CreateMainBannerCommand>
    {
        public CreateMainBannerCommandValidator()
        {
            RuleFor(x => x.MainBannerUrl)
                .NotEmpty().WithMessage("Banner görsel adresi boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Banner görsel adresi en fazla 500 karakter olabilir.")
                .Must(BeValidBannerUrl).WithMessage("Banner görsel adresi geçerli bir bağlantı veya yol olmalıdır.");

            RuleFor(x => x.MainBannerDesc)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
        }

        private static bool BeValidBannerUrl(string? url)
        {
            return ContentUrlRules.IsSafeImageUrl(url);
        }
    }
}
