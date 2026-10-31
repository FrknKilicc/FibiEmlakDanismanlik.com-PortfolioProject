using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingDealValidators
{
    public class CancelListingDealCommandValidator : AbstractValidator<CancelListingDealCommand>
    {
        public CancelListingDealCommandValidator()
        {
            RuleFor(x => x.ListingDealId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir işlem kaydı belirtilmelidir.");

            RuleFor(x => x.DepositOutcome)
                .IsInEnum()
                .WithMessage("Geçerli bir kapora sonucu belirtilmelidir.");
        }
    }
}
