using FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateBankLoanRateCommandValidator : AbstractValidator<UpdateBankLoanRateCommand>
    {
        public UpdateBankLoanRateCommandValidator()
        {
            RuleFor(x => x.BankLoanRateId)
                .GreaterThan(0).WithMessage("Geçerli bir banka oranı kimliği belirtilmelidir.");

            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage("Banka adı zorunludur.")
                .MaximumLength(100).WithMessage("Banka adı en fazla 100 karakter olabilir.");

            When(x => !string.IsNullOrWhiteSpace(x.LogoUrl), () =>
            {
                RuleFor(x => x.LogoUrl)
                    .MaximumLength(500).WithMessage("Logo adresi en fazla 500 karakter olabilir.")
                    .Must(ContentUrlRules.IsSafeImageUrl).WithMessage("Geçerli bir logo adresi girin.");
            });

            RuleFor(x => x.HousingLoanMonthlyRate)
                .InclusiveBetween(0.01m, 10m).WithMessage("Aylık faiz oranı 0,01 ile 10 arasında olmalıdır.")
                .PrecisionScale(6, 4, true).WithMessage("Konut kredisi faiz oranı en fazla 4 ondalık basamak içerebilir.");

            RuleFor(x => x.CommercialLoanMonthlyRate)
                .InclusiveBetween(0.01m, 10m).WithMessage("Aylık faiz oranı 0,01 ile 10 arasında olmalıdır.")
                .PrecisionScale(6, 4, true).WithMessage("Ticari kredi faiz oranı en fazla 4 ondalık basamak içerebilir.");

            RuleFor(x => x.MaxTermMonths)
                .InclusiveBetween(12, 360).WithMessage("Maksimum vade 12 ile 360 ay arasında olmalıdır.");

            RuleFor(x => x.AllocationFeeRate)
                .InclusiveBetween(0m, 5m).WithMessage("Tahsis ücreti oranı 0 ile 5 arasında olmalıdır.")
                .PrecisionScale(6, 4, true).WithMessage("Tahsis ücreti oranı en fazla 4 ondalık basamak içerebilir.");
        }
    }
}
