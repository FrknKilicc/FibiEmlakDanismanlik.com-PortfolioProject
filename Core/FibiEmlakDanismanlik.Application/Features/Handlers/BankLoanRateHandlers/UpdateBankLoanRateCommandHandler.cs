using FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BankLoanRateHandlers
{
    public class UpdateBankLoanRateCommandHandler : IRequestHandler<UpdateBankLoanRateCommand>
    {
        private readonly IRepository<BankLoanRate> _repository;

        public UpdateBankLoanRateCommandHandler(IRepository<BankLoanRate> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBankLoanRateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.BankLoanRateId);
            if (entity == null)
            {
                throw new KeyNotFoundException("Banka oranı bulunamadı.");
            }

            entity.BankName = request.BankName.Trim();
            entity.LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? null : request.LogoUrl.Trim();
            // Güncellemede ProviderKey ve SourceUrl değişmez
            entity.DataSource = "Yönetim panelinden girildi";
            entity.HousingLoanMonthlyRate = request.HousingLoanMonthlyRate;
            entity.CommercialLoanMonthlyRate = request.CommercialLoanMonthlyRate;
            entity.MaxTermMonths = request.MaxTermMonths;
            entity.AllocationFeeRate = request.AllocationFeeRate;
            entity.IsActive = request.IsActive;
            entity.UpdatedDate = TurkeyClock.Now;

            await _repository.UpdateAsync(entity);
        }
    }
}
