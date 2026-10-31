using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Veri senkronizasyonu günlüğü sonucu.</summary>
    public record GetMarketDataSyncLogResult(
        int MarketDataSyncLogId,
        string ProviderKey,
        DateTime StartedAt,
        DateTime FinishedAt,
        bool IsSuccess,
        int AffectedRowCount,
        string? Message);
}
