using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>FİBİ canlı veri senkronizasyonu sabitleri.</summary>
    public static class FibiMarketDataSyncConstants
    {
        // Sağlayıcı anahtarları
        public const string SeedProviderKey = "seed";
        public const string IbbSeismicProviderKey = "ibb-seismic";
        public const string TcmbKfeBulletinProviderKey = "tcmb-kfe";
        public const string EvdsProviderKey = "tcmb-evds";

        // Veri kaynağı metinleri
        public const string IbbSeismicDataSource = "İBB Açık Veri, 7,5 Mw deprem senaryosu";
        public const string IndexAdjustedSeedDataSource = "FİBİ tahmini, TCMB KFE ile güncellendi";
        public const string EvdsDataSource = "TCMB EVDS konut birim fiyatı";

        // Dış kaynak adresleri
        public const string IbbSeismicScenarioCsvUrl = "https://data.ibb.gov.tr/dataset/c13514d9-86b1-4b83-a9b9-1a15cb5f254c/resource/9c3ac492-de4b-4245-b418-7ad3df67a193/download/deprem-senaryosu-analiz-sonuclar.csv";
        public const string IbbBuildingCountCsvUrl = "https://data.ibb.gov.tr/dataset/be3582eb-09d7-42f8-84d3-b3817dc9ab0a/resource/cef193d5-0bd2-4e8d-8a69-275c50288875/download/2017-yl-mahalle-bazl-bina-saylar.csv";
        public const string TcmbKfePageUrl = "https://www.tcmb.gov.tr/wps/wcm/connect/tr/tcmb+tr/main+menu/istatistikler/reel+sektor+istatistikleri/konut+fiyat+endeksi";
        public const string TcmbBaseUrl = "https://www.tcmb.gov.tr";

        // HTTP
        public const string HttpClientName = "fibi-market-data";
        public const int HttpTimeoutSeconds = 30;
        public const string UserAgent = "FibiEmlakMarketDataSync/1.0";

        // Biçim ve doğrulama
        public const int IstanbulPlaceCode = 34;
        public const string NeighborhoodNameSuffix = " MAHALLESİ";
        public const int HistoryQuarterCount = 8;
        public const int MinimumRegionalChangeCount = 10;
        public const decimal MaximumAbsoluteChangePercentage = 200m;
        public const int SyncMessageMaxLength = FibiServiceConstants.SyncMessageMaxLength;
        public const int RecentSyncLogCount = 50;

        // İBB 2017 verisindeki eski ilçe adları → güncel adlar (karşılaştırma anahtarı biçiminde)
        public static readonly IReadOnlyDictionary<string, string> IbbDistrictAliases = new Dictionary<string, string>
        {
            ["EYÜP"] = "EYÜPSULTAN"
        };
        public const string CompactNeighborhoodSuffix = "MAHALLESI";
        public const string CompactNeighborhoodShortSuffix = "MAHALLE";
    }
}
