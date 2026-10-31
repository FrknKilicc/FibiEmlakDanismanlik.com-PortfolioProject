using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries
{
    /// <summary>Son veri senkronizasyonu günlüklerini getiren sorgu.</summary>
    public record GetMarketDataSyncLogsQuery() : IRequest<List<GetMarketDataSyncLogResult>>;
}
