namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Konumun deprem tehlikesi özeti ve kaynağı.</summary>
    public record FibiSeismicSummaryResult(
        string SourceName,
        bool IsOfficialData,
        string HazardLevelLabel,
        decimal? HeavyDamageRatePercentage,
        int? TotalBuildingCount,
        decimal? Pre1980BuildingSharePercentage);
}
