using FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BankLoanRateHandlers
{
    public class CreateBankLoanRateCommandHandler : IRequestHandler<CreateBankLoanRateCommand>
    {
        private readonly IRepository<BankLoanRate> _repository;

        public CreateBankLoanRateCommandHandler(IRepository<BankLoanRate> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBankLoanRateCommand request, CancellationToken cancellationToken)
        {
            var entity = new BankLoanRate
            {
                BankName = request.BankName.Trim(),
                LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl.Trim(),
                ProviderKey = null,
                SourceUrl = null,
                DataSource = "Yönetim panelinden girildi",
                HousingLoanMonthlyRate = request.HousingLoanMonthlyRate,
                CommercialLoanMonthlyRate = request.CommercialLoanMonthlyRate,
                MaxTermMonths = request.MaxTermMonths,
                AllocationFeeRate = request.AllocationFeeRate,
                IsActive = request.IsActive,
                UpdatedDate = DateTime.Now
            };

            await _repository.CreateAsync(entity);
        }
    }
}
