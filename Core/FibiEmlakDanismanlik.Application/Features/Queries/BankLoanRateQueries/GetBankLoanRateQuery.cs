using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.BankLoanRateResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BankLoanRateQueries
{
    /// <summary>Aktif banka kredi faiz oranlarını getiren sorgu.</summary>
    public class GetBankLoanRateQuery : IRequest<List<GetBankLoanRateResult>>
    {
    }
}
