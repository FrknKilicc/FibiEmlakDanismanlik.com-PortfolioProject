using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Bina risk ve kentsel dönüşüm analizi hesaplama sonucu.</summary>
    public class CalculateBuildingRiskResult
    {
        public FibiLocationSummaryResult Location { get; set; } = null!;
        public FibiDataSourceResult DataSource { get; set; } = null!;
        public FibiSeismicSummaryResult Seismic { get; set; } = null!;
        public int StructuralRiskScore { get; set; }
        public string RiskLevel { get; set; } = null!; // "Düşük" / "Orta" / "Yüksek" / "Kritik"
        public string RiskLevelCode { get; set; } = null!; // "low" / "medium" / "high" / "critical"
        public int UrbanTransformationPotentialScore { get; set; }
        public string UrbanTransformationVerdict { get; set; } = null!;
        public List<FibiInsightResult> ActionableInsights { get; set; } = new();
        public string LegalDisclaimer { get; set; } = null!;
    }
}
