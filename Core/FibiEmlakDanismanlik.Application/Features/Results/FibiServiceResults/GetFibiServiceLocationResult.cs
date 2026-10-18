using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>FİBİ hizmetleri için hiyerarşik şehir konum sonucu.</summary>
    public record GetFibiServiceLocationResult(int CityId, string CityName, List<FibiServiceDistrictResult> Districts);

    /// <summary>FİBİ hizmetleri için hiyerarşik ilçe konum sonucu.</summary>
    public record FibiServiceDistrictResult(int DistrictId, string DistrictName, List<FibiServiceNeighborhoodResult> Neighborhoods);

    /// <summary>FİBİ hizmetleri için mahalle konum sonucu.</summary>
    public record FibiServiceNeighborhoodResult(int NeighborhoodId, string NeighborhoodName);
}
