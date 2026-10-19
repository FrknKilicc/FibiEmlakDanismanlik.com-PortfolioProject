using System;
using System.Linq;

namespace FibiEmlakDanismanlik.Application.Constants
{
    public static class ListingGeoConstants
    {
        public const string SourceManual = "manual";
        public const string SourceNominatim = "nominatim";

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
    }
}
