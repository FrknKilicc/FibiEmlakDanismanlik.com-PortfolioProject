using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData.Providers
{
    /// <summary>TCMB Konut Fiyat Endeksi (KFE) aylık bültenini indirip emsal kayıtlarına uygulayan sağlayıcı.</summary>
    public class TcmbKfeBulletinProvider : IMarketDataProvider
    {
        private static readonly Regex KfePdfRegex = new(
            @"href=""(?<u>[^""]*?/KFE\.pdf(?:\?[^""]*)?)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMarketDataSyncRepository _repository;

        public TcmbKfeBulletinProvider(
            IHttpClientFactory httpClientFactory,
            IMarketDataSyncRepository repository)
        {
            _httpClientFactory = httpClientFactory;
            _repository = repository;
        }

        public string Key => FibiMarketDataSyncConstants.TcmbKfeBulletinProviderKey;

        public TimeSpan MinimumInterval => TimeSpan.FromDays(1);

        public int Order => 20;

        public async Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(FibiMarketDataSyncConstants.HttpClientName);

            var pageHtml = await client.GetStringAsync(
                FibiMarketDataSyncConstants.TcmbKfePageUrl,
                cancellationToken);

            var match = KfePdfRegex.Match(pageHtml);
            if (!match.Success)
            {
                return MarketDataSyncOutcome.Failure("KFE bülteni bağlantısı bulunamadı.");
            }

            var pdfUrl = match.Groups["u"].Value.Replace("&amp;", "&");
            if (!pdfUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !pdfUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var baseUri = new Uri(FibiMarketDataSyncConstants.TcmbBaseUrl);
                pdfUrl = new Uri(baseUri, pdfUrl).ToString();
            }

            var pdfBytes = await client.GetByteArrayAsync(pdfUrl, cancellationToken);
            var text = PdfTextExtractor.ExtractLines(pdfBytes);
            var data = KfeBulletinParser.Parse(text);

            if (data == null)
            {
                return MarketDataSyncOutcome.Failure("KFE bülteni ayrıştırılamadı; mevcut veriler korundu.");
            }

            var tracked = await _repository.GetTrackedStatsWithCityAsync();
            var now = DateTime.Now;
            var affectedCount = 0;

            foreach (var item in tracked)
            {
                var provinceKey = item.CityName == null ? null : TurkishTextHelper.ToComparisonKey(item.CityName);
                if (MarketIndexApplier.ApplyBulletin(item.Stat, provinceKey, data, now))
                {
                    affectedCount++;
                }
            }

            if (affectedCount > 0)
            {
                await _repository.SaveChangesAsync();
            }

            return MarketDataSyncOutcome.Success(
                affectedCount,
                affectedCount > 0
                    ? $"{data.Period} bülteni {affectedCount} emsal kaydına uygulandı."
                    : $"{data.Period} bülteni zaten uygulanmış.");
        }
    }
}
