using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    /// <summary>Son veri senkronizasyonu günlüklerini getiren sorguyu işleyen işleyici.</summary>
    public class GetMarketDataSyncLogsQueryHandler : IRequestHandler<GetMarketDataSyncLogsQuery, List<GetMarketDataSyncLogResult>>
    {
        private readonly IMarketDataSyncRepository _repository;

        public GetMarketDataSyncLogsQueryHandler(IMarketDataSyncRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetMarketDataSyncLogResult>> Handle(GetMarketDataSyncLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _repository.GetRecentSyncLogsAsync(FibiMarketDataSyncConstants.RecentSyncLogCount);

            return logs.Select(x => new GetMarketDataSyncLogResult(
                x.MarketDataSyncLogId,
                x.ProviderKey,
                x.StartedAt,
                x.FinishedAt,
                x.IsSuccess,
                x.AffectedRowCount,
                x.Message
            )).ToList();
        }
    }
}
