using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.BankLoanRateResults
{
    /// <summary>Banka kredi faiz oranı sonuç modeli.</summary>
    public class GetBankLoanRateResult
    {
        public int BankLoanRateId { get; set; }

        public string BankName { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string? ProviderKey { get; set; }

        public string DataSource { get; set; } = null!;

        public decimal HousingLoanMonthlyRate { get; set; }

        public decimal CommercialLoanMonthlyRate { get; set; }

        public int MaxTermMonths { get; set; }

        public decimal AllocationFeeRate { get; set; }

        public bool IsActive { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
