using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands
{
    public class CreateBankLoanRateCommand : IRequest
    {
        public string BankName { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public decimal HousingLoanMonthlyRate { get; set; }
        public decimal CommercialLoanMonthlyRate { get; set; }
        public int MaxTermMonths { get; set; }
        public decimal AllocationFeeRate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
