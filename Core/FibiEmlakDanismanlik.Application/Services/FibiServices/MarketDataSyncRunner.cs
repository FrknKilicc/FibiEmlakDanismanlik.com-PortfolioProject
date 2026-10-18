using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Services.FibiServices
{
    /// <summary>Dış piyasa veri sağlayıcılarını sırayla ve güvenli çalıştıran çekirdek servis.</summary>
    public class MarketDataSyncRunner : IMarketDataSyncRunner
    {
        private readonly IEnumerable<IMarketDataProvider> _providers;
        private readonly IMarketDataSyncRepository _repository;

        public MarketDataSyncRunner(
            IEnumerable<IMarketDataProvider> providers,
            IMarketDataSyncRepository repository)
        {
            _providers = providers;
            _repository = repository;
        }

        public async Task<IReadOnlyList<(string ProviderKey, MarketDataSyncOutcome Outcome)>> RunAllAsync(bool force, CancellationToken cancellationToken)
        {
            var orderedProviders = _providers.OrderBy(p => p.Order).ToList();
            var results = new List<(string ProviderKey, MarketDataSyncOutcome Outcome)>(orderedProviders.Count);

            foreach (var provider in orderedProviders)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!force)
                {
                    var lastSuccess = await _repository.GetLastSuccessfulSyncAsync(provider.Key);
                    if (lastSuccess.HasValue && (DateTime.Now - lastSuccess.Value) < provider.MinimumInterval)
                    {
                        results.Add((provider.Key, MarketDataSyncOutcome.Skipped("Bekleme süresi dolmadı.")));
                        continue;
                    }
                }

                var outcome = await RunProviderAsync(provider, cancellationToken);
                results.Add((provider.Key, outcome));
            }

            return results;
        }

        public async Task<MarketDataSyncOutcome?> RunByKeyAsync(string providerKey, CancellationToken cancellationToken)
        {
            var provider = _providers.FirstOrDefault(p => string.Equals(p.Key, providerKey, StringComparison.OrdinalIgnoreCase));
            if (provider == null)
                return null;

            return await RunProviderAsync(provider, cancellationToken);
        }

        private async Task<MarketDataSyncOutcome> RunProviderAsync(IMarketDataProvider provider, CancellationToken ct)
        {
            var startedAt = DateTime.Now;
            MarketDataSyncOutcome outcome;

            try
            {
                outcome = await provider.SyncAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                outcome = MarketDataSyncOutcome.Failure($"{ex.GetType().Name}: {ex.Message}");
            }

            var finishedAt = DateTime.Now;

            if (!outcome.IsSuccess)
            {
                _repository.DiscardPendingChanges();
            }

            if (!outcome.IsSkipped)
            {
                try
                {
                    var message = outcome.Message;
                    if (!string.IsNullOrEmpty(message) && message.Length > FibiMarketDataSyncConstants.SyncMessageMaxLength)
                    {
                        message = message.Substring(0, FibiMarketDataSyncConstants.SyncMessageMaxLength);
                    }

                    var log = new MarketDataSyncLog
                    {
                        ProviderKey = provider.Key,
                        StartedAt = startedAt,
                        FinishedAt = finishedAt,
                        IsSuccess = outcome.IsSuccess,
                        AffectedRowCount = outcome.AffectedRowCount,
                        Message = message
                    };

                    await _repository.AddSyncLogAsync(log);
                }
                catch
                {
                    // Log hatası çalıştırma sonucunu değiştirmesin
                }
            }

            return outcome;
        }
    }
}
