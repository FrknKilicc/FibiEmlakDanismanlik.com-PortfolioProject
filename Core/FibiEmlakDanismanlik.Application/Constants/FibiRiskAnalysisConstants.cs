namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>FİBİ bina risk analizi ve kentsel dönüşüm puanlama sabitleri.</summary>
    public static class FibiRiskAnalysisConstants
    {
        // Yapım yılı puanları
        public const int Pre1999Points = 40;
        public const int Between2000And2007Points = 25;
        public const int Between2008And2018Points = 12;
        public const int Post2018Points = 0;

        // Zemin sınıfı puanları
        public const int SoilLowPoints = 5;
        public const int SoilMediumPoints = 15;
        public const int SoilHighPoints = 25;
        public const int SoilVeryHighPoints = 35;

        // Yapısal risk ek puanları ve eşikler
        public const int SoftStoryPoints = 12; // Zemin kat ticari (yumuşak kat)
        public const int AdjacentBuildingPoints = 8;
        public const int AdjacentBuildingFloorThreshold = 5; // Bitişik nizam ve TotalFloors > 5
        public const int MasonryPoints = 10; // Yığma yapı
        public const int NoBasementPoints = 4; // Bodrum/otopark yok (rijitlik kaybı)
        public const int LowFloorOnWeakSoilPoints = 3; // CurrentFloor <= 0 ve zemin High/VeryHigh

        public const int MaxScore = 100;

        // Risk seviyesi eşikleri (0–29 Düşük, 30–49 Orta, 50–69 Yüksek, 70+ Kritik)
        public const int MediumRiskThreshold = 30;
        public const int HighRiskThreshold = 50;
        public const int CriticalRiskThreshold = 70;

        // Risk seviyesi metin ve kodları
        public const string RiskLowLabel = "Düşük";
        public const string RiskLowCode = "low";
        public const string RiskMediumLabel = "Orta";
        public const string RiskMediumCode = "medium";
        public const string RiskHighLabel = "Yüksek";
        public const string RiskHighCode = "high";
        public const string RiskCriticalLabel = "Kritik";
        public const string RiskCriticalCode = "critical";

        // Kentsel dönüşüm yaş katsayıları
        public const decimal Pre1999AgeFactor = 1.0m;
        public const decimal Between2000And2007AgeFactor = 0.8m;
        public const decimal Between2008And2018AgeFactor = 0.35m;
        public const decimal Post2018AgeFactor = 0.1m;

        // Kentsel dönüşüm ağırlık ve oranları
        public const decimal AppetiteWeight = 0.6m;
        public const decimal PriceWeight = 0.4m;
        public const decimal PremiumPriceRatio = 1.25m;

        public const int HighTransformationThreshold = 70;
        public const int MediumTransformationThreshold = 45;

        // Kentsel dönüşüm kanaat metinleri
        public const string HighTransformationVerdict = "Yüksek Kentsel Dönüşüm / Arsa Payı Yatırım Fırsatı";
        public const string MediumTransformationVerdict = "Orta Düzey Dönüşüm Potansiyeli";
        public const string LowTransformationVerdict = "Düşük Dönüşüm Potansiyeli";

        // Yasal uyarı
        public const string LegalDisclaimer = "Bu analiz; seçilen mahallenin dış piyasa ve zemin verileri ile sizin verdiğiniz bilgiler kullanılarak üretilen genel bir ön değerlendirmedir. Resmi bina performans analizi, zemin etüdü veya riskli yapı tespiti yerine geçmez. Kesin sonuç için yetkili kuruluşlarca yapılacak teknik inceleme gereklidir.";
    }
}
