using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FluentValidation;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateBlogCommandValidator : AbstractValidator<CreateBlogCommand>
    {
        public CreateBlogCommandValidator()
        {
            RuleFor(x => x.BlogTitle)
                .NotEmpty().WithMessage("Blog başlığı boş bırakılamaz.")
                .MaximumLength(200).WithMessage("Blog başlığı en fazla 200 karakter olabilir.");

            RuleFor(x => x.BlogDescription)
                .NotEmpty().WithMessage("Blog açıklaması boş bırakılamaz.")
                .MaximumLength(100000).WithMessage("Blog açıklaması en fazla 100000 karakter olabilir.");

            RuleFor(x => x.BlogImgUrl)
                .NotEmpty().WithMessage("Blog görsel adresi boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Blog görsel adresi en fazla 500 karakter olabilir.")
                .Must(BeValidImageUrl).WithMessage("Blog görsel adresi geçerli bir bağlantı veya yol olmalıdır.");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Geçerli bir yazar belirtilmelidir.");

            RuleFor(x => x.BlogCategoryId)
                .Must(id => !id.HasValue || id.Value > 0)
                .WithMessage("Blog kategori kimliği 0'dan büyük olmalıdır.");

            RuleFor(x => x.CreatedDate)
                .Must(BeValidCreatedDate)
                .WithMessage("Oluşturulma tarihi 01.01.2000 ile bugün + 1 yıl arasında olmalıdır.");

            RuleFor(x => x.Tags)
                .Must(t => BlogTagNormalizer.Normalize(t).Count <= 10)
                .WithMessage("En fazla 10 etiket eklenebilir.");

            RuleForEach(x => x.Tags)
                .Must(t => t == null || t.Trim().Length <= 50)
                .WithMessage("Bir etiket en fazla 50 karakter olabilir.");
        }

        private static bool BeValidImageUrl(string? url)
        {
            return ContentUrlRules.IsSafeImageUrl(url);
        }

        private static bool BeValidCreatedDate(DateTime date)
        {
            if (date < new DateTime(2000, 1, 1))
                return false;

            var maxDay = (DateTime.UtcNow.Date > DateTime.Today ? DateTime.UtcNow.Date : DateTime.Today).AddYears(1);
            return date.Date <= maxDay;
        }
    }
}
