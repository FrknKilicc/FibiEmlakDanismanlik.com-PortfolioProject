using FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BankLoanRateHandlers
{
    public class RemoveBankLoanRateCommandHandler : IRequestHandler<RemoveBankLoanRateCommand>
    {
        private readonly IRepository<BankLoanRate> _repository;

        public RemoveBankLoanRateCommandHandler(IRepository<BankLoanRate> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveBankLoanRateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Banka oranı bulunamadı.");
            }

            await _repository.RemoveAsync(entity);
        }
    }
}
