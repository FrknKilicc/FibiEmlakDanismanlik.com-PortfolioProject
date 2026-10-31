using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.BankLoanRateResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BankLoanRateQueries
{
    /// <summary>Banka kredi faiz oranlarını getiren sorgu.</summary>
    public class GetBankLoanRateQuery : IRequest<List<GetBankLoanRateResult>>
    {
        public bool IncludeInactive { get; set; }

        public GetBankLoanRateQuery()
        {
        }

        public GetBankLoanRateQuery(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }
    }
}
