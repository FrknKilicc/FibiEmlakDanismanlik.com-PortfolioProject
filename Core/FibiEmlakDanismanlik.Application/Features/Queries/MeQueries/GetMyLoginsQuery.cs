using FibiEmlakDanismanlik.Application.Features.Results.MeResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MeQueries
{
    /// <summary>
    /// Giriş yapmış çalışanın son giriş denemelerini getiren sorgu.
    /// </summary>
    public class GetMyLoginsQuery : IRequest<List<GetMyLoginResult>>
    {
    }
}
