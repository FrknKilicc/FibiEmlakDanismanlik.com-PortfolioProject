using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>Dış kaynak senkronizasyon çalıştırma kaydı.</summary>
    public class MarketDataSyncLog
    {
        [Key]
        public int MarketDataSyncLogId { get; set; }

        public string ProviderKey { get; set; } = null!;

        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }

        public bool IsSuccess { get; set; }

        public int AffectedRowCount { get; set; }

        public string? Message { get; set; }
    }
}
