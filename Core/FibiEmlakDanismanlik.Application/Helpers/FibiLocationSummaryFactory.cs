using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>FİBİ konum özeti üretim ve biçimlendirme fabrikası.</summary>
    public static class FibiLocationSummaryFactory
    {
        public static FibiLocationSummaryResult FromNeighborhood(Neighborhood neighborhood)
        {
            var cityName = TurkishTextHelper.ToTurkishTitleCase(neighborhood.District.City.Name);
            var districtName = TurkishTextHelper.ToTurkishTitleCase(neighborhood.District.Name);
            var neighborhoodName = TurkishTextHelper.ToTurkishTitleCase(neighborhood.Name);

            return new FibiLocationSummaryResult(
                neighborhood.District.City.CityId,
                cityName,
                neighborhood.District.DistrictId,
                districtName,
                neighborhood.NeighborhoodId,
                neighborhoodName);
        }

        public static FibiLocationSummaryResult ToDisplay(FibiLocationSummaryResult raw)
        {
            return raw with
            {
                CityName = TurkishTextHelper.ToTurkishTitleCase(raw.CityName),
                DistrictName = TurkishTextHelper.ToTurkishTitleCase(raw.DistrictName),
                NeighborhoodName = TurkishTextHelper.ToTurkishTitleCase(raw.NeighborhoodName)
            };
        }
    }
}
