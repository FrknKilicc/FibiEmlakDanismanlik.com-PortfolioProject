namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>FİBİ modülleri için ortak konum özeti sonucu.</summary>
    public record FibiLocationSummaryResult(
        int CityId,
        string CityName,
        int DistrictId,
        string DistrictName,
        int NeighborhoodId,
        string NeighborhoodName);
}
