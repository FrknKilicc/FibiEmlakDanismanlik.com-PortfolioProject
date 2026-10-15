namespace FibiEmlakDanismanlik.Domain.Interfaces
{
    // İl/ilçe bilgisi taşıyan ilan entity'leri: metin + Cities/Districts ID'leri
    public interface ILocatedListing
    {
        string City { get; set; }
        string District { get; set; }
        int? CityId { get; set; }
        int? DistrictId { get; set; }
    }
}
