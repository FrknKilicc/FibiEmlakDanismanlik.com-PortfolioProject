namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>FİBİ bölge analizi motoru katsayı ve eşik sabitleri.</summary>
    public static class FibiNeighborhoodAnalysisConstants
    {
        // Segment katsayıları
        public const decimal StandardApartmentCoefficient = 1.0m;
        public const decimal NewOrGatedCommunityCoefficient = 1.18m;
        public const decimal CommercialCoefficient = 1.25m;

        // Projeksiyon katsayı ve ağırlıkları
        public const decimal GrowthRateWeight = 0.6m;
        public const decimal DevelopmentFactorWeight = 15m;
        public const decimal ConservativeMultiplier = 0.6m;
        public const decimal OptimisticMultiplier = 1.35m;
        public const int ProjectionQuarterCount = 4;

        // Amortisman eşikleri ve kanaat etiketleri
        public const decimal HighRentYieldMaxYears = 15m;
        public const decimal BalancedMaxYears = 20m;
        public const string HighRentYieldVerdict = "Yüksek Kira Verimi";
        public const string BalancedVerdict = "Dengeli";
        public const string ValueGrowthVerdict = "Değer Artışı Odaklı Bölge";

        // Yatırım notu hesaplama sabitleri
        public const decimal BestAmortizationYears = 12m;
        public const decimal WorstAmortizationYears = 30m;
        public const decimal MaxGrowthRateForFullScore = 50m;
        public const decimal AmortizationWeight = 0.40m;
        public const decimal GrowthWeight = 0.35m;
        public const decimal AppetiteWeight = 0.25m;

        // Yatırım notu harf notu eşikleri
        public const int GradeAPlusThreshold = 80;
        public const int GradeAThreshold = 65;
        public const int GradeBThreshold = 50;

        // Yasal uyarı
        public const string Disclaimer = "Fiyat bantları ve projeksiyonlar dış piyasa emsal verilerinden türetilmiş tahmini aralıklardır; bağlayıcı bir değerleme veya yatırım tavsiyesi değildir. Mülke özel değer için uzman ekspertiz raporu alınmalıdır.";
    }
}
