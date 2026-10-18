namespace FibiEmlakDanismanlik.Infrastructure.MarketData
{
    /// <summary>İBB deprem senaryosu dosya adları ve doğrulama sabitleri.</summary>
    internal static class IbbSeismicFiles
    {
        public const string ScenarioFileName = "deprem-senaryosu-analiz-sonuclari.csv";
        public const string BuildingCountFileName = "mahalle-bazli-bina-sayilari-2017.csv";
        public const int MinimumExpectedRecordCount = 500;
    }
}
