using System;

namespace FibiEmlakDanismanlik.AdminUI.Models.Dashboard;

/// <summary>
/// Veri senkronizasyonu günlüğü API yanıt modeli.
/// </summary>
public sealed class SyncLogDto
{
    public int MarketDataSyncLogId { get; set; }
    public string ProviderKey { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public bool IsSuccess { get; set; }
    public int AffectedRowCount { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Kontrol paneli uyarısı için son başarısız senkronizasyon kaydı.
/// </summary>
public sealed class SyncFailureItem
{
    public string ProviderKey { get; set; } = string.Empty;
    public DateTime FinishedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
