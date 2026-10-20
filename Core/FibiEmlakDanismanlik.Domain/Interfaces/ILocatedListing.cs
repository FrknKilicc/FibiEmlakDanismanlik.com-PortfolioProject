namespace FibiEmlakDanismanlik.Domain.Interfaces
{
    // İl/ilçe/mahalle bilgisi taşıyan ilan entity'leri: metin + Cities/Districts ID'leri
    public interface ILocatedListing
    {
        string City { get; set; }
        string District { get; set; }
        string Neighborhood { get; set; }
        int? CityId { get; set; }
        int? DistrictId { get; set; }
        int? NeighborhoodId { get; set; }
    }
}
