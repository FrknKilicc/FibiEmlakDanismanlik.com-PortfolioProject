using System;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>Dış piyasa mahalle emsal verileri.</summary>
    public class ExternalNeighborhoodMarketStat
    {
        [Key]
        public int ExternalNeighborhoodMarketStatId { get; set; }

        public int NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; } = null!;

        /// <summary>Dış piyasa baz satılık m² fiyatı (TL).</summary>
        public decimal BaseSalePricePerSqm { get; set; }

        /// <summary>Dış piyasa baz aylık kiralık m² fiyatı (TL).</summary>
        public decimal BaseRentPricePerSqm { get; set; }

        /// <summary>Nitelikli üst bant çarpanı.</summary>
        public decimal AgencyMarginMultiplier { get; set; }

        /// <summary>Alt emsal bandı sapma yüzdesi.</summary>
        public decimal BandSpreadPercentage { get; set; }

        /// <summary>Son 1 yıl artış %.</summary>
        public decimal Last1YearGrowthRate { get; set; }

        /// <summary>Son 3 yıl kümülatif artış %.</summary>
        public decimal Last3YearsCompoundGrowthRate { get; set; }

        /// <summary>Bölge gelişim katsayısı (1.0–1.5).</summary>
        public decimal RegionalDevelopmentFactor { get; set; }

        public SoilRiskClass SoilRiskClass { get; set; }

        /// <summary>Kentsel dönüşüm iştah puanı (0–100).</summary>
        public int UrbanTransformationAppetiteScore { get; set; }

        /// <summary>Son 8 çeyrek fiyat geçmişi JSON.</summary>
        public string HistoricalQuarterlyPricesJson { get; set; } = null!;

        public DateTime LastUpdatedDate { get; set; }
    }
}
