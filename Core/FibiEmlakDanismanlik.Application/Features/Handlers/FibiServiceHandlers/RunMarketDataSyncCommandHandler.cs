using FibiEmlakDanismanlik.Application.Features.Commands.FibiServiceCommands;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    /// <summary>Veri senkronizasyonu komutunu işleyen işleyici.</summary>
    public class RunMarketDataSyncCommandHandler : IRequestHandler<RunMarketDataSyncCommand, List<RunMarketDataSyncResult>?>
    {
        private readonly IMarketDataSyncRunner _runner;

        public RunMarketDataSyncCommandHandler(IMarketDataSyncRunner runner)
        {
            _runner = runner;
        }

        public async Task<List<RunMarketDataSyncResult>?> Handle(RunMarketDataSyncCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ProviderKey))
            {
                var outcomes = await _runner.RunAllAsync(force: true, cancellationToken);
                return outcomes.Select(x => new RunMarketDataSyncResult(
                    x.ProviderKey,
                    x.Outcome.IsSuccess,
                    x.Outcome.IsSkipped,
                    x.Outcome.AffectedRowCount,
                    x.Outcome.Message
                )).ToList();
            }

            var outcome = await _runner.RunByKeyAsync(request.ProviderKey, cancellationToken);
            if (outcome == null)
            {
                return null;
            }

            return new List<RunMarketDataSyncResult>
            {
                new(request.ProviderKey, outcome.IsSuccess, outcome.IsSkipped, outcome.AffectedRowCount, outcome.Message)
            };
        }
    }
}
