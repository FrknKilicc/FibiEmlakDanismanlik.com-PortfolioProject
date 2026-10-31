using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// İş kuralı ayarları sabitleri ve varsayılan değerleri.
    /// </summary>
    public static class BusinessSettingConstants
    {
        public const int SingletonId = 1;

        public const decimal DefaultSaleBuyerCommissionRate = 2m;
        public const decimal DefaultSaleSellerCommissionRate = 2m;
        public const decimal DefaultRentCommissionMonths = 1m;
        public const int DefaultBestDealMaxPerUsageType = BestDealConstants.MaxPerUsageType;
        public const int DefaultListingAlertMaxActivePerPhone = ListingAlertConstants.MaxActivePerPhone;

        public const decimal MaxRentCommissionMonths = 12m;
        public const int MinBestDealMax = 1;
        public const int MaxBestDealMax = 20;
        public const int MinAlertMaxPerPhone = 1;
        public const int MaxAlertMaxPerPhone = 50;

        /// <summary>
        /// Tekil kayıt bulunamadığında kullanılacak varsayılan ayar nesnesini üretir.
        /// </summary>
        public static BusinessSetting CreateDefault() => new BusinessSetting
        {
            BusinessSettingId = SingletonId,
            SaleBuyerCommissionRate = DefaultSaleBuyerCommissionRate,
            SaleSellerCommissionRate = DefaultSaleSellerCommissionRate,
            RentCommissionMonths = DefaultRentCommissionMonths,
            BestDealMaxPerUsageType = DefaultBestDealMaxPerUsageType,
            ListingAlertMaxActivePerPhone = DefaultListingAlertMaxActivePerPhone
        };
    }
}
