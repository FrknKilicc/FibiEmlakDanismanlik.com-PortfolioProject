using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingDealValidators
{
    public class AddDealShareCommandValidator : AbstractValidator<AddDealShareCommand>
    {
        public AddDealShareCommandValidator()
        {
            RuleFor(x => x.ListingDealId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir işlem kaydı belirtilmelidir.");

            RuleFor(x => x.AgentId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir çalışan seçilmelidir.");

            RuleFor(x => x)
                .Must(x => x.SharePercent.HasValue || x.Amount.HasValue)
                .WithMessage("Prim için oran veya tutar girin.");

            RuleFor(x => x.SharePercent)
                .GreaterThan(0)
                .LessThanOrEqualTo(ListingDealConstants.MaxSharePercent)
                .When(x => x.SharePercent.HasValue)
                .WithMessage($"Prim oranı 0'dan büyük ve en fazla {ListingDealConstants.MaxSharePercent} olabilir.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .When(x => x.Amount.HasValue)
                .WithMessage("Prim tutarı sıfırdan büyük olmalıdır.");

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Note))
                .WithMessage("Açıklama en fazla 500 karakter olabilir.");
        }
    }
}
