using System;
using System.Linq;

namespace FibiEmlakDanismanlik.Application.Constants
{
    public static class ListingGeoConstants
    {
        public const string SourceManual = "manual";
        public const string SourceNominatim = "nominatim";

        public const string PrecisionBuilding = "building";
        public const string PrecisionNeighborhood = "neighborhood";
        public const string PrecisionDistrict = "district";
        public const string PrecisionCity = "city";

        public static readonly string[] ApproximatePrecisions = { PrecisionNeighborhood, PrecisionDistrict, PrecisionCity };

        public static bool IsApproximate(string? precision)
        {
            if (string.IsNullOrWhiteSpace(precision))
                return false;

            return ApproximatePrecisions.Any(p => string.Equals(p, precision, StringComparison.OrdinalIgnoreCase));
        }

        public const string CategoryHousing = "housing";
        public const string CategoryLand = "land";
        public const string CategoryCommercial = "commercial";

        public const string GeocodeProviderKey = "listing-geocode";
        public const string NominatimSearchUrl = "https://nominatim.openstreetmap.org/search";
        public const string NominatimCountryCode = "tr";
        public const int GeocodeMaxRequestsPerRun = 60;
        public const int GeocodeRequestDelayMilliseconds = 1100;
        public const int GeocodeCandidateLimit = 500;
        public const string CountryName = "Türkiye";
    }
}
