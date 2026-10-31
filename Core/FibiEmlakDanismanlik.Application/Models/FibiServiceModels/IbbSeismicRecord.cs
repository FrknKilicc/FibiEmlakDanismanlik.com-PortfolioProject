namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>İBB deprem senaryosu ve bina sayısı CSV'lerinin UAVT ile birleştirilmiş mahalle satırı.</summary>
    public record IbbSeismicRecord(
        string DistrictName,
        string NeighborhoodName,
        int UavtCode,
        int BuildingsBefore1980Count,
        int Buildings1980To2000Count,
        int BuildingsAfter2000Count,
        int VeryHeavilyDamagedBuildingCount,
        int HeavilyDamagedBuildingCount,
        int ModeratelyDamagedBuildingCount,
        int LightlyDamagedBuildingCount,
        int EstimatedFatalityCount,
        int TemporaryShelterNeedCount);
}
