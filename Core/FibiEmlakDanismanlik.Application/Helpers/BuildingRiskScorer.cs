using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>Bina yapısal risk ve kentsel dönüşüm potansiyeli saf hesaplama motoru.</summary>
    public static class BuildingRiskScorer
    {
        public static int CalculateStructuralScore(CalculateBuildingRiskQuery input, SoilRiskClass soil)
        {
            var score = 0;

            // Yapım yılı puanı
            score += input.ConstructionYearRange switch
            {
                ConstructionYearRange.Pre1999 => FibiRiskAnalysisConstants.Pre1999Points,
                ConstructionYearRange.Between2000And2007 => FibiRiskAnalysisConstants.Between2000And2007Points,
                ConstructionYearRange.Between2008And2018 => FibiRiskAnalysisConstants.Between2008And2018Points,
                ConstructionYearRange.Post2018NewRegulation => FibiRiskAnalysisConstants.Post2018Points,
                _ => 0
            };

            // Zemin sınıfı puanı
            score += soil switch
            {
                SoilRiskClass.Low => FibiRiskAnalysisConstants.SoilLowPoints,
                SoilRiskClass.Medium => FibiRiskAnalysisConstants.SoilMediumPoints,
                SoilRiskClass.High => FibiRiskAnalysisConstants.SoilHighPoints,
                SoilRiskClass.VeryHigh => FibiRiskAnalysisConstants.SoilVeryHighPoints,
                _ => 0
            };

            // Zemin kat ticari alan (yumuşak kat riski)
            if (input.HasGroundFloorCommercial)
                score += FibiRiskAnalysisConstants.SoftStoryPoints;

            // Bitişik nizam ve kat eşiği (çekiçleme etkisi)
            if (input.IsAdjacentBuilding && input.TotalFloors > FibiRiskAnalysisConstants.AdjacentBuildingFloorThreshold)
                score += FibiRiskAnalysisConstants.AdjacentBuildingPoints;

            // Yapı tipi
            if (input.StructureType == StructureType.Masonry)
                score += FibiRiskAnalysisConstants.MasonryPoints;

            // Bodrum/otopark bulunmaması (rijitlik kaybı)
            if (!input.HasBasementOrParking)
                score += FibiRiskAnalysisConstants.NoBasementPoints;

            // Zayıf zeminde bodrum/zemin kat
            if (input.CurrentFloor <= 0 && IsWeakSoil(soil))
                score += FibiRiskAnalysisConstants.LowFloorOnWeakSoilPoints;

            return Math.Clamp(score, 0, FibiRiskAnalysisConstants.MaxScore);
        }

        public static (string Label, string Code) GetRiskLevel(int score)
        {
            if (score >= FibiRiskAnalysisConstants.CriticalRiskThreshold)
                return (FibiRiskAnalysisConstants.RiskCriticalLabel, FibiRiskAnalysisConstants.RiskCriticalCode);

            if (score >= FibiRiskAnalysisConstants.HighRiskThreshold)
                return (FibiRiskAnalysisConstants.RiskHighLabel, FibiRiskAnalysisConstants.RiskHighCode);

            if (score >= FibiRiskAnalysisConstants.MediumRiskThreshold)
                return (FibiRiskAnalysisConstants.RiskMediumLabel, FibiRiskAnalysisConstants.RiskMediumCode);

            return (FibiRiskAnalysisConstants.RiskLowLabel, FibiRiskAnalysisConstants.RiskLowCode);
        }

        public static int CalculateTransformationScore(
            ConstructionYearRange yearRange,
            int appetiteScore,
            decimal salePricePerSqm,
            decimal cityAverageSalePrice)
        {
            var ageFactor = yearRange switch
            {
                ConstructionYearRange.Pre1999 => FibiRiskAnalysisConstants.Pre1999AgeFactor,
                ConstructionYearRange.Between2000And2007 => FibiRiskAnalysisConstants.Between2000And2007AgeFactor,
                ConstructionYearRange.Between2008And2018 => FibiRiskAnalysisConstants.Between2008And2018AgeFactor,
                ConstructionYearRange.Post2018NewRegulation => FibiRiskAnalysisConstants.Post2018AgeFactor,
                _ => 0m
            };

            var priceFactor = cityAverageSalePrice > 0m
                ? Math.Min(1m, salePricePerSqm / (cityAverageSalePrice * FibiRiskAnalysisConstants.PremiumPriceRatio))
                : 0m;

            var rawScore = ageFactor * (FibiRiskAnalysisConstants.AppetiteWeight * appetiteScore + FibiRiskAnalysisConstants.PriceWeight * priceFactor * 100m);

            return Math.Clamp((int)Math.Round(rawScore, MidpointRounding.AwayFromZero), 0, FibiRiskAnalysisConstants.MaxScore);
        }

        public static string GetTransformationVerdict(int score)
        {
            if (score >= FibiRiskAnalysisConstants.HighTransformationThreshold)
                return FibiRiskAnalysisConstants.HighTransformationVerdict;

            if (score >= FibiRiskAnalysisConstants.MediumTransformationThreshold)
                return FibiRiskAnalysisConstants.MediumTransformationVerdict;

            return FibiRiskAnalysisConstants.LowTransformationVerdict;
        }

        public static List<FibiInsightResult> BuildInsights(CalculateBuildingRiskQuery input, SoilRiskClass soil, int transformationScore)
        {
            var insights = new List<FibiInsightResult>();

            // 1. Bina yapım dönemi
            switch (input.ConstructionYearRange)
            {
                case ConstructionYearRange.Pre1999:
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Danger,
                        "Bina 1999 öncesi deprem yönetmeliğiyle inşa edilmiştir; taşıyıcı sistemin güncel yönetmelik kriterlerini karşılamama olasılığı yüksektir."));
                    break;

                case ConstructionYearRange.Between2000And2007:
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Warning,
                        "Bina 2000–2007 döneminde inşa edilmiştir; beton kalitesi ve donatı detayları için yerinde inceleme önerilir."));
                    break;

                case ConstructionYearRange.Post2018NewRegulation:
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Success,
                        "Bina 2018 Türkiye Bina Deprem Yönetmeliği döneminde inşa edilmiştir."));
                    break;
            }

            // 2. Zemin ve Ticari (Yumuşak Kat)
            var soilCode = GetSoilCode(soil);
            var isWeak = IsWeakSoil(soil);

            if (isWeak && input.HasGroundFloorCommercial)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Danger,
                    $"Seçilen mahalle {soilCode} zemin sınıfındadır ve binada zemin kat ticari alan (yumuşak kat) bulunmaktadır."));
            }
            else
            {
                if (isWeak)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Danger,
                        $"Seçilen mahalle {soilCode} zemin sınıfındadır; zemin büyütme etkisi deprem yükünü artırabilir."));
                }

                if (input.HasGroundFloorCommercial)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Warning,
                        "Zemin katta ticari alan bulunması yumuşak kat ve kolon kesme riski oluşturabilir."));
                }
            }

            // 3. Sağlam zemin
            if (soil == SoilRiskClass.Low)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Success,
                    "Seçilen mahalle Z1 (sağlam) zemin sınıfındadır."));
            }

            // 4. Bitişik nizam ve kat sayısı
            if (input.IsAdjacentBuilding && input.TotalFloors > FibiRiskAnalysisConstants.AdjacentBuildingFloorThreshold)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Warning,
                    $"Bitişik nizam ve {input.TotalFloors} katlı yapıda deprem sırasında komşu binayla çarpışma (çekiçleme) etkisi oluşabilir."));
            }

            // 5. Yığma yapı
            if (input.StructureType == StructureType.Masonry)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Warning,
                    "Yığma yapılar yatay deprem yüklerine karşı betonarme ve çeliğe göre daha hassastır."));
            }

            // 6. Bodrum kat eksikliği
            if (!input.HasBasementOrParking)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Info,
                    "Bodrum kat bulunmaması, binanın zemine oturma rijitliğini azaltabilir."));
            }

            // 7. Zayıf zeminde bodrum veya zemin kat
            if (input.CurrentFloor <= 0 && isWeak)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Info,
                    "Zemin veya bodrum kattaki daireler zayıf zeminde sıvılaşma ve su basması riskine daha açıktır."));
            }

            // 8. Kentsel dönüşüm potansiyeli
            if (transformationScore >= FibiRiskAnalysisConstants.HighTransformationThreshold)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Success,
                    "Bölgedeki m² değeri ve müteahhit dönüşüm iştahı yüksek olduğundan kentsel dönüşüm fizibilitesi güçlüdür."));
            }

            return insights;
        }

        private static bool IsWeakSoil(SoilRiskClass soil) =>
            soil == SoilRiskClass.High || soil == SoilRiskClass.VeryHigh;

        private static string GetSoilCode(SoilRiskClass soil) => soil switch
        {
            SoilRiskClass.Low => "Z1",
            SoilRiskClass.Medium => "Z2",
            SoilRiskClass.High => "Z3",
            SoilRiskClass.VeryHigh => "Z4",
            _ => "Bilinmeyen"
        };
    }
}
