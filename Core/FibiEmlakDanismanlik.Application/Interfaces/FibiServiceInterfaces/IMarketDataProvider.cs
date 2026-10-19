using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    /// <summary>Dış emsal/faiz/deprem verisi sağlayıcısı. Hata durumunda DB'yi değiştirmeden Failure döner veya fırlatır; çalıştırıcı yakalar.</summary>
    public interface IMarketDataProvider
    {
        string Key { get; }

        /// <summary>İki başarılı çalıştırma arasındaki en kısa süre.</summary>
        TimeSpan MinimumInterval { get; }

        /// <summary>Küçük olan önce çalışır (başlangıç verisi diğerlerinden önce yüklenmeli).</summary>
        int Order { get; }

        Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken);
    }
}
