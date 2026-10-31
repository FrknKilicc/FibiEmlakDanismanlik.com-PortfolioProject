namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>FİBİ emsal verisi kapsam etiketleri ve deprem tehlikesi eşikleri.</summary>
    public static class FibiMarketDataConstants
    {
        // Kapsam kodları ve etiketleri
        public const string NeighborhoodScopeCode = "neighborhood";
        public const string DistrictScopeCode = "district";
        public const string CityScopeCode = "city";
        public const string CountryScopeCode = "country";
        public const string NeighborhoodScopeLabel = "Mahalle verisi";
        public const string DistrictScopeLabel = "İlçe ortalaması";
        public const string CityScopeLabel = "İl ortalaması";
        public const string CountryScopeLabel = "Türkiye ortalaması";

        // İBB senaryosu: (çok ağır + ağır hasarlı bina) / toplam bina yüzdesi eşikleri.
        // İstanbul'daki 959 mahallenin dağılımı: medyan %2,1, üst çeyrek %5,8, üst %10 %10,7.
        public const decimal SeismicMediumDamageRateThreshold = 1.5m;
        public const decimal SeismicHighDamageRateThreshold = 4m;
        public const decimal SeismicVeryHighDamageRateThreshold = 8m;
        public const decimal OldBuildingShareWarningPercentage = 40m;

        // Deprem tehlikesi seviye etiketleri
        public const string HazardLowLabel = "Düşük";
        public const string HazardMediumLabel = "Orta";
        public const string HazardHighLabel = "Yüksek";
        public const string HazardVeryHighLabel = "Çok yüksek";
        public const string HazardUnknownLabel = "Bilinmiyor";

        // Deprem verisi kaynak metinleri
        public const string RegionalSoilEstimateSourceFormat = "Bölgesel zemin tahmini ({0})";
        public const string NoSeismicDataSource = "Bu konum için deprem/zemin verisi bulunamadı";
    }
}
