using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Tek bir mahallenin bölge analizi sonucu.</summary>
    public class NeighborhoodAnalysisItemResult
    {
        public FibiLocationSummaryResult Location { get; set; } = null!;
        public FibiPriceBandResult PriceBand { get; set; } = null!;
        public decimal AmortizationYears { get; set; }
        public decimal AnnualRentYieldPercentage { get; set; }
        public string AmortizationVerdict { get; set; } = null!;
        public decimal Expected12MonthGrowthRate { get; set; }
        public decimal Conservative12MonthGrowthRate { get; set; }
        public decimal Optimistic12MonthGrowthRate { get; set; }
        public decimal Last1YearGrowthRate { get; set; }
        public decimal Last3YearsCompoundGrowthRate { get; set; }
        public int InvestmentScore { get; set; }
        public string InvestmentGrade { get; set; } = null!;
        public List<FibiQuarterlyPriceResult> HistoricalPrices { get; set; } = new();
        public List<FibiProjectionPointResult> Projection { get; set; } = new();
    }
}
