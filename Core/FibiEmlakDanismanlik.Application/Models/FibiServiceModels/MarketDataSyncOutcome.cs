namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>Bir sağlayıcı çalıştırmasının sonucu.</summary>
    public record MarketDataSyncOutcome(bool IsSuccess, bool IsSkipped, int AffectedRowCount, string Message)
    {
        public static MarketDataSyncOutcome Success(int affectedRowCount, string message) => new(true, false, affectedRowCount, message);
        public static MarketDataSyncOutcome Skipped(string message) => new(true, true, 0, message);
        public static MarketDataSyncOutcome Failure(string message) => new(false, false, 0, message);
    }
}
