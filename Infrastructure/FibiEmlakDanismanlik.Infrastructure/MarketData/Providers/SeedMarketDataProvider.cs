using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Infrastructure.MarketData.Seed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData.Providers
{
    /// <summary>Gömülü başlangıç emsal ve İBB deprem senaryosu verilerini yükleyen sağlayıcı.</summary>
    public class SeedMarketDataProvider : IMarketDataProvider
    {
        private readonly IMarketDataSyncRepository _repository;

        public SeedMarketDataProvider(IMarketDataSyncRepository repository)
        {
            _repository = repository;
        }

        public string Key => FibiMarketDataSyncConstants.SeedProviderKey;

        public TimeSpan MinimumInterval => TimeSpan.FromDays(1);

        public int Order => 0;

        public async Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var inserted = await _repository.InsertMissingSeedStatsAsync(
                FibiMarketStatSeedData.All,
                FibiMarketStatSeedData.SeedPeriod,
                now);

            var seismic = 0;
            if (!await _repository.HasSeismicScenarioDataAsync())
            {
                var scenarioCsv = EmbeddedResourceReader.ReadText(IbbSeismicFiles.ScenarioFileName, TurkishEncoding.Windows1254);
                var buildingCsv = EmbeddedResourceReader.ReadText(IbbSeismicFiles.BuildingCountFileName, TurkishEncoding.Windows1254);
                var records = IbbSeismicCsvParser.Parse(scenarioCsv, buildingCsv);

                seismic = await _repository.UpsertSeismicScenariosAsync(
                    records,
                    FibiMarketDataSyncConstants.IbbSeismicDataSource,
                    now);
            }

            return MarketDataSyncOutcome.Success(
                inserted + seismic,
                $"{inserted} emsal kaydı, {seismic} deprem senaryosu kaydı eklendi.");
        }
    }
}
