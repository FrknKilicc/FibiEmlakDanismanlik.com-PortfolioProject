using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Hesaplamada kullanılan emsal verisinin kapsamı ve kaynağı.</summary>
    public record FibiDataSourceResult(string ScopeCode, string ScopeLabel, string SourceName, DateTime LastUpdatedDate, bool IsEstimated);
}
