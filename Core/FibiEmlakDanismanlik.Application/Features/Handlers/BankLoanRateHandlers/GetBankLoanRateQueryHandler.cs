using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.BankLoanRateQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BankLoanRateResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BankLoanRateHandlers
{
    /// <summary>Aktif banka kredi faiz oranlarını listeleyen sorgu işleyicisi.</summary>
    public class GetBankLoanRateQueryHandler : IRequestHandler<GetBankLoanRateQuery, List<GetBankLoanRateResult>>
    {
        private readonly IRepository<BankLoanRate> _repository;

        public GetBankLoanRateQueryHandler(IRepository<BankLoanRate> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetBankLoanRateResult>> Handle(GetBankLoanRateQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            return values
                .Where(x => x.IsActive)
                .OrderBy(x => x.HousingLoanMonthlyRate)
                .Select(x => new GetBankLoanRateResult
                {
                    BankLoanRateId = x.BankLoanRateId,
                    BankName = x.BankName,
                    LogoUrl = x.LogoUrl,
                    DataSource = x.DataSource,
                    HousingLoanMonthlyRate = x.HousingLoanMonthlyRate,
                    CommercialLoanMonthlyRate = x.CommercialLoanMonthlyRate,
                    MaxTermMonths = x.MaxTermMonths,
                    AllocationFeeRate = x.AllocationFeeRate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToList();
        }
    }
}
