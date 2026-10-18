using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    public interface IMarketDataSyncRunner
    {
        /// <summary>Tüm sağlayıcıları Order sırasıyla çalıştırır; force=false iken MinimumInterval dolmamış olanları atlar.</summary>
        Task<IReadOnlyList<(string ProviderKey, MarketDataSyncOutcome Outcome)>> RunAllAsync(bool force, CancellationToken cancellationToken);

        /// <summary>Tek sağlayıcıyı zorla çalıştırır. Anahtar bulunamazsa null.</summary>
        Task<MarketDataSyncOutcome?> RunByKeyAsync(string providerKey, CancellationToken cancellationToken);
    }
}
