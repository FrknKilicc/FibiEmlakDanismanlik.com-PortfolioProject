using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>Banka konut/ticari kredi faiz oranları.</summary>
    public class BankLoanRate
    {
        [Key]
        public int BankLoanRateId { get; set; }

        public string BankName { get; set; } = null!;

        public string? LogoUrl { get; set; }

        /// <summary>Konut kredisi aylık faiz oranı (% — 2.49 = %2,49).</summary>
        public decimal HousingLoanMonthlyRate { get; set; }

        /// <summary>Ticari kredi aylık faiz oranı (%).</summary>
        public decimal CommercialLoanMonthlyRate { get; set; }

        public int MaxTermMonths { get; set; }

        /// <summary>Kredi tahsis ücreti (%).</summary>
        public decimal AllocationFeeRate { get; set; }

        public bool IsActive { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
