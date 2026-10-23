using System.Linq;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>
    /// Satış ve kiralama işlem komisyonu ve çalışan prim hesaplama yardımcı metotları.
    /// </summary>
    public static class ListingDealCalculator
    {
        /// <summary>
        /// İşlem türüne göre komisyon tutarlarını ve oranlarını kurallara uygun biçimde belirler.
        /// </summary>
        public static void ApplyCommission(ListingDeal d)
        {
            if (d.DealType == DealType.Sale)
            {
                if (d.BuyerCommissionRate.HasValue && !d.BuyerCommissionAmount.HasValue)
                {
                    d.BuyerCommissionAmount = ListingDealConstants.Percent(d.AgreedPrice, d.BuyerCommissionRate.Value);
                }

                if (d.SellerCommissionRate.HasValue && !d.SellerCommissionAmount.HasValue)
                {
                    d.SellerCommissionAmount = ListingDealConstants.Percent(d.AgreedPrice, d.SellerCommissionRate.Value);
                }
            }
            else if (d.DealType == DealType.Rent)
            {
                d.BuyerCommissionRate = null;
                d.SellerCommissionRate = null;

                if (!d.BuyerCommissionAmount.HasValue && !d.SellerCommissionAmount.HasValue)
                {
                    d.BuyerCommissionAmount = d.AgreedPrice;
                }
            }
        }

        /// <summary>
        /// Yüzdelik prim oranı (SharePercent) belirtilmiş olan payların tutarlarını toplam komisyon üzerinden yeniden hesaplar.
        /// </summary>
        public static void RecalculateShares(ListingDeal d)
        {
            if (d.Shares == null) return;

            foreach (var share in d.Shares)
            {
                if (share.SharePercent.HasValue)
                {
                    var newAmount = ListingDealConstants.Percent(d.TotalCommission, share.SharePercent.Value);
                    if (newAmount != share.Amount)
                    {
                        if (share.PaymentStatus == DealSharePaymentStatus.Paid)
                        {
                            throw new ValidationException(new[]
                            {
                                new ValidationFailure("BuyerCommissionAmount", "Ödenmiş prim var; komisyonu değiştirmeden önce o primin ödemesini geri alın.")
                            });
                        }

                        if (share.PaymentStatus == DealSharePaymentStatus.Pending)
                        {
                            share.Amount = newAmount;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// İşleme ait prim paylarının toplamının toplam komisyon tutarını aşıp aşmadığını denetler; aşarsa doğrulama hatası fırlatır.
        /// </summary>
        public static void EnsureSharesWithinCommission(ListingDeal d, string field = "Amount")
        {
            var totalShares = d.Shares != null ? d.Shares.Sum(s => s.Amount) : 0m;
            if (totalShares > d.TotalCommission)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(field, "Dağıtılan primlerin toplamı komisyonu aşamaz.")
                });
            }
        }
    }
}
