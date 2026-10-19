using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>İBB Açık Veri deprem senaryosu (7,5 Mw gece senaryosu) ve 2017 mahalle bina sayıları.</summary>
    public class NeighborhoodSeismicScenarioStat
    {
        [Key]
        public int NeighborhoodSeismicScenarioStatId { get; set; }

        public int NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; } = null!;

        /// <summary>İBB mahalle UAVT kodu.</summary>
        public int UavtCode { get; set; }

        public int BuildingsBefore1980Count { get; set; }
        public int Buildings1980To2000Count { get; set; }
        public int BuildingsAfter2000Count { get; set; }

        public int VeryHeavilyDamagedBuildingCount { get; set; }
        public int HeavilyDamagedBuildingCount { get; set; }
        public int ModeratelyDamagedBuildingCount { get; set; }
        public int LightlyDamagedBuildingCount { get; set; }

        public int EstimatedFatalityCount { get; set; }
        public int TemporaryShelterNeedCount { get; set; }

        public string DataSource { get; set; } = null!;

        public DateTime LastUpdatedDate { get; set; }
    }
}
