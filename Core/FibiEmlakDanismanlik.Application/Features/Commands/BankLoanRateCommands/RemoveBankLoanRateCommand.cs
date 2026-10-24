using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands
{
    public class RemoveBankLoanRateCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveBankLoanRateCommand(int id)
        {
            Id = id;
        }
    }
}
