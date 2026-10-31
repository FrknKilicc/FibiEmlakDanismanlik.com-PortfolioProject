using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>Bina yapısal risk ve kentsel dönüşüm potansiyeli saf hesaplama motoru.</summary>
    public static class BuildingRiskScorer
    {
        public static int CalculateStructuralScore(CalculateBuildingRiskQuery input, SoilRiskClass? hazardLevel)
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

            // Deprem tehlikesi / zemin puanı
            score += hazardLevel switch
            {
                SoilRiskClass.Low => FibiRiskAnalysisConstants.SoilLowPoints,
                SoilRiskClass.Medium => FibiRiskAnalysisConstants.SoilMediumPoints,
                SoilRiskClass.High => FibiRiskAnalysisConstants.SoilHighPoints,
                SoilRiskClass.VeryHigh => FibiRiskAnalysisConstants.SoilVeryHighPoints,
                null => FibiRiskAnalysisConstants.SoilUnknownPoints,
                _ => FibiRiskAnalysisConstants.SoilUnknownPoints
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
            if (input.CurrentFloor <= 0 && IsWeakSoil(hazardLevel))
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

        public static List<FibiInsightResult> BuildInsights(CalculateBuildingRiskQuery input, SeismicHazardAssessment seismic, int transformationScore)
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

            // 2 & 3. Deprem tehlikesi / Zemin ve Ticari (Yumuşak Kat)
            AddSeismicInsights(insights, input, seismic);

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
            if (input.CurrentFloor <= 0 && IsWeakSoil(seismic.HazardLevel))
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

        private static void AddSeismicInsights(List<FibiInsightResult> insights, CalculateBuildingRiskQuery input, SeismicHazardAssessment seismic)
        {
            var level = seismic.HazardLevel;
            var isWeak = IsWeakSoil(level);
            var summary = seismic.Summary;
            var softStoryHandled = false;

            if (summary.IsOfficialData)
            {
                var trCulture = CultureInfo.GetCultureInfo("tr-TR");
                var rate = summary.HeavyDamageRatePercentage?.ToString("N1", trCulture);
                var label = summary.HazardLevelLabel;

                if (isWeak)
                {
                    var message = $"İBB deprem senaryosuna göre mahallenizdeki binaların %{rate} kadarında ağır veya çok ağır hasar bekleniyor (deprem tehlikesi: {label}).";
                    if (input.HasGroundFloorCommercial)
                    {
                        message += " Binada zemin kat ticari alan (yumuşak kat) bulunması bu riski artırır.";
                        softStoryHandled = true;
                    }
                    insights.Add(new FibiInsightResult(FibiInsightSeverities.Danger, message));
                }
                else if (level == SoilRiskClass.Medium)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Warning,
                        $"İBB deprem senaryosuna göre mahallenizdeki binaların %{rate} kadarında ağır veya çok ağır hasar bekleniyor."));
                }
                else if (level == SoilRiskClass.Low)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Success,
                        $"İBB deprem senaryosunda mahallenizde ağır hasar beklenen bina oranı düşüktür (%{rate})."));
                }

                if (summary.Pre1980BuildingSharePercentage >= FibiMarketDataConstants.OldBuildingShareWarningPercentage)
                {
                    var share = summary.Pre1980BuildingSharePercentage?.ToString("N1", trCulture);
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Info,
                        $"Mahallenizdeki binaların %{share} kadarı 1980 öncesinde yapılmıştır (İBB 2017 bina envanteri)."));
                }
            }
            else if (level.HasValue)
            {
                var soilCode = GetSoilCode(level.Value);

                if (isWeak && input.HasGroundFloorCommercial)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Danger,
                        $"Bölgesel tahmine göre seçilen konum {soilCode} zemin sınıfındadır ve binada zemin kat ticari alan (yumuşak kat) bulunmaktadır."));
                    softStoryHandled = true;
                }
                else
                {
                    if (isWeak)
                    {
                        insights.Add(new FibiInsightResult(
                            FibiInsightSeverities.Danger,
                            $"Bölgesel tahmine göre seçilen konum {soilCode} zemin sınıfındadır; zemin büyütme etkisi deprem yükünü artırabilir."));
                    }
                }

                if (level.Value == SoilRiskClass.Low)
                {
                    insights.Add(new FibiInsightResult(
                        FibiInsightSeverities.Success,
                        "Bölgesel tahmine göre seçilen konum Z1 (sağlam) zemin sınıfındadır."));
                }
            }
            else
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Info,
                    "Bu konum için resmi zemin veya deprem senaryosu verisi bulunamadı; deprem tehlikesi orta kabul edildi. Adresinizin tehlike değerini AFAD Türkiye Deprem Tehlike Haritası'ndan sorgulayabilirsiniz."));
            }

            if (input.HasGroundFloorCommercial && !softStoryHandled)
            {
                insights.Add(new FibiInsightResult(
                    FibiInsightSeverities.Warning,
                    "Zemin katta ticari alan bulunması yumuşak kat ve kolon kesme riski oluşturabilir."));
            }
        }

        private static bool IsWeakSoil(SoilRiskClass? level) =>
            level == SoilRiskClass.High || level == SoilRiskClass.VeryHigh;

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
