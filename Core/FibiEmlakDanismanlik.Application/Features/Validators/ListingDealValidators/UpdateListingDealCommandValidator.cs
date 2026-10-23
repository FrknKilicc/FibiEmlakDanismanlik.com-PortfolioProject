using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingDealValidators
{
    public class UpdateListingDealCommandValidator : AbstractValidator<UpdateListingDealCommand>
    {
        public UpdateListingDealCommandValidator()
        {
            RuleFor(x => x.ListingDealId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir işlem kaydı belirtilmelidir.");

            RuleFor(x => x.AgreedPrice)
                .GreaterThan(0)
                .WithMessage("Anlaşılan bedel sıfırdan büyük olmalıdır.");

            RuleFor(x => x.DepositAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.DepositAmount.HasValue)
                .WithMessage("Kapora tutarı negatif olamaz.");

            RuleFor(x => x.DepositDate)
                .NotNull()
                .When(x => x.DepositAmount.HasValue && x.DepositAmount.Value > 0)
                .WithMessage("Kapora tarihi girilmelidir.");

            RuleFor(x => x.BuyerCommissionRate)
                .InclusiveBetween(0, ListingDealConstants.MaxCommissionRate)
                .When(x => x.BuyerCommissionRate.HasValue)
                .WithMessage($"Alıcı komisyon oranı 0 ile {ListingDealConstants.MaxCommissionRate} arasında olmalıdır.");

            RuleFor(x => x.SellerCommissionRate)
                .InclusiveBetween(0, ListingDealConstants.MaxCommissionRate)
                .When(x => x.SellerCommissionRate.HasValue)
                .WithMessage($"Satıcı komisyon oranı 0 ile {ListingDealConstants.MaxCommissionRate} arasında olmalıdır.");

            RuleFor(x => x.BuyerCommissionAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.BuyerCommissionAmount.HasValue)
                .WithMessage("Alıcı komisyon tutarı negatif olamaz.");

            RuleFor(x => x.SellerCommissionAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.SellerCommissionAmount.HasValue)
                .WithMessage("Satıcı komisyon tutarı negatif olamaz.");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Müşteri adı zorunludur.")
                .MaximumLength(100)
                .WithMessage("Müşteri adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.CustomerPhone)
                .MaximumLength(20)
                .When(x => !string.IsNullOrEmpty(x.CustomerPhone))
                .WithMessage("Müşteri telefon numarası en fazla 20 karakter olabilir.");

            RuleFor(x => x.Notes)
                .MaximumLength(2000)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Notlar en fazla 2000 karakter olabilir.");
        }
    }
}
