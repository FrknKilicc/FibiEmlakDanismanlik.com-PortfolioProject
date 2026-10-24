using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.BusinessSettingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.BusinessSettingValidators
{
    /// <summary>
    /// İş kuralı ayarlarını güncelleme komutu doğrulayıcısı.
    /// </summary>
    public class UpdateBusinessSettingsCommandValidator : AbstractValidator<UpdateBusinessSettingsCommand>
    {
        public UpdateBusinessSettingsCommandValidator()
        {
            RuleFor(x => x.SaleBuyerCommissionRate)
                .InclusiveBetween(0, ListingDealConstants.MaxCommissionRate)
                .WithMessage($"Alıcı komisyon oranı 0 ile {ListingDealConstants.MaxCommissionRate} arasında olmalıdır.")
                .PrecisionScale(5, 2, true)
                .WithMessage("En fazla iki ondalık basamak girilebilir.");

            RuleFor(x => x.SaleSellerCommissionRate)
                .InclusiveBetween(0, ListingDealConstants.MaxCommissionRate)
                .WithMessage($"Satıcı komisyon oranı 0 ile {ListingDealConstants.MaxCommissionRate} arasında olmalıdır.")
                .PrecisionScale(5, 2, true)
                .WithMessage("En fazla iki ondalık basamak girilebilir.");

            RuleFor(x => x.RentCommissionMonths)
                .InclusiveBetween(0, BusinessSettingConstants.MaxRentCommissionMonths)
                .WithMessage($"Kiralama komisyonu 0 ile {BusinessSettingConstants.MaxRentCommissionMonths} kira arasında olmalıdır.")
                .PrecisionScale(4, 2, true)
                .WithMessage("En fazla iki ondalık basamak girilebilir.");

            RuleFor(x => x.BestDealMaxPerUsageType)
                .InclusiveBetween(BusinessSettingConstants.MinBestDealMax, BusinessSettingConstants.MaxBestDealMax)
                .WithMessage($"Vitrin sınırı {BusinessSettingConstants.MinBestDealMax} ile {BusinessSettingConstants.MaxBestDealMax} arasında olmalıdır.");

            RuleFor(x => x.ListingAlertMaxActivePerPhone)
                .InclusiveBetween(BusinessSettingConstants.MinAlertMaxPerPhone, BusinessSettingConstants.MaxAlertMaxPerPhone)
                .WithMessage($"Telefon başına aktif kayıt sınırı {BusinessSettingConstants.MinAlertMaxPerPhone} ile {BusinessSettingConstants.MaxAlertMaxPerPhone} arasında olmalıdır.");
        }
    }
}
