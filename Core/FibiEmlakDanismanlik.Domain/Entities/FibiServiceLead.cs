using System;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>FİBİ Hizmetler analiz ekranlarından gelen danışmanlık talepleri.</summary>
    public class FibiServiceLead
    {
        [Key]
        public int FibiServiceLeadId { get; set; }

        public FibiServiceModule SourceModule { get; set; }

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Email { get; set; }

        /// <summary>Talebin ilgili olduğu mahalle (opsiyonel).</summary>
        public int? NeighborhoodId { get; set; }
        public Neighborhood? Neighborhood { get; set; }

        /// <summary>Kullanıcının girdi ve sonuç özeti JSON.</summary>
        public string? CalculationSummaryJson { get; set; }

        public string? CustomerNote { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsContacted { get; set; }
    }
}
