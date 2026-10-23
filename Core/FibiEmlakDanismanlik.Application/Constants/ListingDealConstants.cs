using System;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// Satış / kiralama işlemleri ve hak ediş (prim) sabitleri ile yardımcı işlevleri.
    /// </summary>
    public static class ListingDealConstants
    {
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;

        public const decimal MaxCommissionRate = 100m;
        public const decimal MaxSharePercent = 100m;

        /// <summary>
        /// İşlem durumunun kullanıcı dostu Türkçe metin karşılığı.
        /// </summary>
        public static string StatusLabel(DealStatus s) => s switch
        {
            DealStatus.DepositReceived => "Kapora alındı",
            DealStatus.Completed => "Tamamlandı",
            DealStatus.Cancelled => "İptal",
            _ => s.ToString()
        };

        /// <summary>
        /// İşlem türünün kullanıcı dostu Türkçe metin karşılığı.
        /// </summary>
        public static string DealTypeLabel(DealType t) => t switch
        {
            DealType.Sale => "Satış",
            DealType.Rent => "Kiralama",
            _ => t.ToString()
        };

        /// <summary>
        /// Prim ödeme durumunun kullanıcı dostu Türkçe metin karşılığı.
        /// </summary>
        public static string PaymentStatusLabel(DealSharePaymentStatus s) => s switch
        {
            DealSharePaymentStatus.Pending => "Bekliyor",
            DealSharePaymentStatus.Paid => "Ödendi",
            _ => s.ToString()
        };

        /// <summary>
        /// Belirtilen tutar üzerinden yüzdelik prim / komisyon tutarını hesaplar ve 2 basamağa yuvarlar.
        /// </summary>
        public static decimal Percent(decimal baseAmount, decimal percent) =>
            Math.Round(baseAmount * percent / 100m, 2, MidpointRounding.AwayFromZero);
    }
}
