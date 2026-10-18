namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Veri senkronizasyonu çalıştırma sonucu.</summary>
    public record RunMarketDataSyncResult(
        string ProviderKey,
        bool IsSuccess,
        bool IsSkipped,
        int AffectedRowCount,
        string Message);
}
