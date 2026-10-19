namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>appsettings "FibiMarketDataSync" bölümü.</summary>
    public class MarketDataSyncOptions
    {
        public const string SectionName = "FibiMarketDataSync";
        public bool Enabled { get; set; } = true;
        public int IntervalHours { get; set; } = 12;
        public int InitialDelaySeconds { get; set; } = 30;
        /// <summary>Opsiyonel. Boşsa EVDS sağlayıcısı sessizce atlanır. Yalnızca User Secrets / ortam değişkeninde tutulur.</summary>
        public string? EvdsApiKey { get; set; }
        public string EvdsBaseUrl { get; set; } = "https://evds2.tcmb.gov.tr/service/evds/";
    }
}
