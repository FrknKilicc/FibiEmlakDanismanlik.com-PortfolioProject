using FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands;
using FluentValidation;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateSocialMediaCommandValidator : AbstractValidator<UpdateSocialMediaCommand>
    {
        public UpdateSocialMediaCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir sosyal medya kimliği belirtilmelidir.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Sosyal medya adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Sosyal medya adı en fazla 50 karakter olabilir.");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Sosyal medya bağlantısı boş bırakılamaz.")
                .MaximumLength(300).WithMessage("Sosyal medya bağlantısı en fazla 300 karakter olabilir.")
                .Must(BeValidHttpUrl).WithMessage("Adres http:// veya https:// ile başlayan geçerli bir bağlantı olmalıdır.");

            RuleFor(x => x.Icon)
                .MaximumLength(100).WithMessage("İkon en fazla 100 karakter olabilir.")
                .Matches(@"^[a-zA-Z0-9 \-]*$").WithMessage("İkon yalnızca harf, rakam, boşluk ve tire içerebilir.")
                .When(x => !string.IsNullOrEmpty(x.Icon));

            RuleFor(x => x.Order)
                .InclusiveBetween(0, 1000).WithMessage("Sıra 0 ile 1000 arasında olmalıdır.");
        }

        private static bool BeValidHttpUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var u) &&
                   (u.Scheme == "http" || u.Scheme == "https");
        }
    }
}
