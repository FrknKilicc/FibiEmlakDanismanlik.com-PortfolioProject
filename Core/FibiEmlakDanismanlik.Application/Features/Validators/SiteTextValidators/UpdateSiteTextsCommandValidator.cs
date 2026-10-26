using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.SiteTextCommands;
using FluentValidation;
using FluentValidation.Results;

namespace FibiEmlakDanismanlik.Application.Features.Validators.SiteTextValidators
{
    /// <summary>
    /// Site metni güncelleme komutu için doğrulama: bilinmeyen anahtar, uzunluk ve sayı aralığı.
    /// </summary>
    public class UpdateSiteTextsCommandValidator : AbstractValidator<UpdateSiteTextsCommand>
    {
        public UpdateSiteTextsCommandValidator()
        {
            RuleFor(x => x.Values).NotNull().WithMessage("Güncellenecek metin gönderilmelidir.");

            RuleFor(x => x.Values).Custom((values, context) =>
            {
                if (values == null) return;
                foreach (var (key, raw) in values)
                {
                    var def = SiteTextDefinitions.Find(key);
                    if (def == null)
                    {
                        context.AddFailure(new ValidationFailure(key, $"Bilinmeyen site metni: {key}"));
                        continue;
                    }

                    var value = (raw ?? string.Empty).Trim();
                    if (value.Length > def.MaxLength)
                    {
                        context.AddFailure(new ValidationFailure(key, $"{def.Label} en fazla {def.MaxLength} karakter olabilir."));
                    }

                    if (def.Kind == SiteTextDefinitions.KindNumber &&
                        (!int.TryParse(value, out var n) || n < def.Min || n > def.Max))
                    {
                        context.AddFailure(new ValidationFailure(key, $"{def.Label} {def.Min} ile {def.Max} arasında bir tam sayı olmalıdır."));
                    }
                }
            });
        }
    }
}
