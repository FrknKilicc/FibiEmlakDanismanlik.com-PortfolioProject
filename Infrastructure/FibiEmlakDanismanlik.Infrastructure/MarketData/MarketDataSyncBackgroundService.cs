using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>Periyodik olarak dış piyasa veri sağlayıcılarını çalıştıran arka plan servisi.</summary>
    public class MarketDataSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<MarketDataSyncOptions> _options;
        private readonly ILogger<MarketDataSyncBackgroundService> _logger;

        public MarketDataSyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<MarketDataSyncOptions> options,
            ILogger<MarketDataSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _options.Value;
            if (!options.Enabled)
            {
                _logger.LogInformation("Canlı veri senkronizasyonu kapalı.");
                return;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(options.InitialDelaySeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await RunCycleAsync(stoppingToken);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(Math.Max(1, options.IntervalHours)));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunCycleAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // İptal durumunda sessizce çık
            }
        }

        private async Task RunCycleAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMarketDataSyncRunner>();
                var results = await runner.RunAllAsync(force: false, stoppingToken);

                foreach (var (providerKey, outcome) in results)
                {
                    if (outcome.IsSuccess)
                    {
                        _logger.LogInformation(
                            "Sağlayıcı: {ProviderKey}, Etkilenen Satır: {AffectedRowCount}, Mesaj: {Message}",
                            providerKey,
                            outcome.AffectedRowCount,
                            outcome.Message);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Sağlayıcı: {ProviderKey}, Etkilenen Satır: {AffectedRowCount}, Mesaj: {Message}",
                            providerKey,
                            outcome.AffectedRowCount,
                            outcome.Message);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // İptal durumu
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veri senkronizasyonu döngüsünde beklenmeyen hata oluştu.");
            }
        }
    }
}
