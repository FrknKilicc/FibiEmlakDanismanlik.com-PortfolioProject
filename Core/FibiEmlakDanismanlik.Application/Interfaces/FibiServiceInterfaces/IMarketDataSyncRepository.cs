using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    /// <summary>Dış kaynak senkronizasyonunun veritabanı işlemleri.</summary>
    public interface IMarketDataSyncRepository
    {
        Task<DateTime?> GetLastSuccessfulSyncAsync(string providerKey);

        Task AddSyncLogAsync(MarketDataSyncLog log);

        /// <summary>Kapsam anahtarı (Türkiye / il / il+ilçe) DB'de olmayan kayıtları ekler; mevcutlara dokunmaz. Eklenen sayıyı döner.</summary>
        Task<int> InsertMissingSeedStatsAsync(IReadOnlyList<MarketStatSeedRecord> records, string seedPeriod, DateTime now);

        Task<bool> HasSeismicScenarioDataAsync();

        /// <summary>İstanbul mahallelerini ilçe + mahalle adıyla eşleştirip ekler/günceller. Etkilenen sayıyı döner.</summary>
        Task<int> UpsertSeismicScenariosAsync(IReadOnlyList<IbbSeismicRecord> records, string dataSource, DateTime now);

        /// <summary>Takip edilen emsal kayıtları ve il plaka kodları (Türkiye kaydı için null).</summary>
        Task<List<(ExternalMarketStat Stat, int? CityPlaceCode, string? CityName)>> GetTrackedStatsWithCityAsync();

        Task<BankLoanRate?> GetTrackedBankRateAsync(string providerKey);

        Task<List<MarketDataSyncLog>> GetRecentSyncLogsAsync(int count);

        Task<int> SaveChangesAsync();

        /// <summary>Kaydedilmemiş tüm değişiklikleri bırakır (başarısız çalıştırmadan sonra).</summary>
        void DiscardPendingChanges();
    }
}
