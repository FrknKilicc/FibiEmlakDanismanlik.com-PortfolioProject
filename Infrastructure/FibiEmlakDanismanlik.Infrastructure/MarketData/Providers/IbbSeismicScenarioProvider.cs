using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData.Providers
{
    /// <summary>İBB Açık Veri Portalı'ndan güncel deprem senaryosu ve bina verilerini çeken sağlayıcı.</summary>
    public class IbbSeismicScenarioProvider : IMarketDataProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMarketDataSyncRepository _repository;

        public IbbSeismicScenarioProvider(
            IHttpClientFactory httpClientFactory,
            IMarketDataSyncRepository repository)
        {
            _httpClientFactory = httpClientFactory;
            _repository = repository;
        }

        public string Key => FibiMarketDataSyncConstants.IbbSeismicProviderKey;

        public TimeSpan MinimumInterval => TimeSpan.FromDays(30);

        public int Order => 10;

        public async Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(FibiMarketDataSyncConstants.HttpClientName);

            var scenarioBytes = await client.GetByteArrayAsync(
                FibiMarketDataSyncConstants.IbbSeismicScenarioCsvUrl,
                cancellationToken);

            var buildingBytes = await client.GetByteArrayAsync(
                FibiMarketDataSyncConstants.IbbBuildingCountCsvUrl,
                cancellationToken);

            var scenarioCsv = TurkishEncoding.Windows1254.GetString(scenarioBytes);
            var buildingCsv = TurkishEncoding.Windows1254.GetString(buildingBytes);

            var records = IbbSeismicCsvParser.Parse(scenarioCsv, buildingCsv);

            if (records.Count < IbbSeismicFiles.MinimumExpectedRecordCount)
            {
                return MarketDataSyncOutcome.Failure($"İBB verisi beklenenden az kayıt içeriyor: {records.Count}");
            }

            var affected = await _repository.UpsertSeismicScenariosAsync(
                records,
                FibiMarketDataSyncConstants.IbbSeismicDataSource,
                DateTime.Now);

            return MarketDataSyncOutcome.Success(affected, $"{affected} mahalle deprem senaryosu güncellendi.");
        }
    }
}
