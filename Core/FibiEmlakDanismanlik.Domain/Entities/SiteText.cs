using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Sitede panelden düzenlenebilen metin/sayı ayarı (anahtar-değer). Tanımlar ve varsayılanlar kodda: SiteTextDefinitions.
    /// </summary>
    public class SiteText
    {
        [Key]
        public int SiteTextId { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = string.Empty;
        public DateTime UpdatedAtUtc { get; set; }
        public int? UpdatedByAgentId { get; set; }
    }
}
