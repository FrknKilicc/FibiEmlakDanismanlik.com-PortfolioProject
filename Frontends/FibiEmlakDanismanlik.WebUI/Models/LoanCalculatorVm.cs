using System.Collections.Generic;
using FibiEmlakDanismanlik.Dto.BankLoanRateDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    /// <summary>Kredi taksit hesaplayıcı görünüm modeli.</summary>
    public class LoanCalculatorVm
    {
        public decimal Price { get; set; }

        public bool IsHousing { get; set; }

        public List<ResultBankLoanRateDto> Rates { get; set; } = new();
    }
}
