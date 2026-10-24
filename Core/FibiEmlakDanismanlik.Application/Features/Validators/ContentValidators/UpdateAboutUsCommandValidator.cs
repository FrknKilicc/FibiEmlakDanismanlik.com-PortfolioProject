using FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands;
using FluentValidation;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateAboutUsCommandValidator : AbstractValidator<UpdateAboutUsCommand>
    {
        public UpdateAboutUsCommandValidator()
        {
            RuleFor(x => x.AboutUsId)
                .GreaterThan(0).WithMessage("Geçerli bir Hakkımızda kimliği belirtilmelidir.");

            RuleFor(x => x.AboutUsTitle)
                .NotEmpty().WithMessage("Başlık boş bırakılamaz.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.AboutUsDesc)
                .NotEmpty().WithMessage("Açıklama boş bırakılamaz.")
                .MaximumLength(20000).WithMessage("Açıklama en fazla 20000 karakter olabilir.");

            RuleFor(x => x.AboutUsImgUrl)
                .MaximumLength(500).WithMessage("Görsel adresi en fazla 500 karakter olabilir.")
                .Must(BeValidImageUrl).WithMessage("Görsel adresi geçerli bir bağlantı veya yol olmalıdır.");
        }

        private static bool BeValidImageUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            if (url.Contains(':'))
            {
                return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                       (uri.Scheme == "http" || uri.Scheme == "https");
            }

            return true;
        }
    }
}
