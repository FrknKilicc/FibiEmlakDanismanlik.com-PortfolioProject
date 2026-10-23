using System;

namespace FibiEmlakDanismanlik.Dto.BankLoanRateDtos
{
    /// <summary>Banka kredi faiz oranı sonuç DTO'su.</summary>
    public class ResultBankLoanRateDto
    {
        public int BankLoanRateId { get; set; }

        public string BankName { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string DataSource { get; set; } = null!;

        public decimal HousingLoanMonthlyRate { get; set; }

        public decimal CommercialLoanMonthlyRate { get; set; }

        public int MaxTermMonths { get; set; }

        public decimal AllocationFeeRate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
