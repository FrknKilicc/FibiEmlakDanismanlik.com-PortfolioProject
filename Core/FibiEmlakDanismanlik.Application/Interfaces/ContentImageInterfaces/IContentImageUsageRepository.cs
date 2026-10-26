using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.ContentImageInterfaces
{
    /// <summary>
    /// Veritabanındaki içerik görsellerine referans verebilecek tüm alanları tarayan repository arayüzü.
    /// </summary>
    public interface IContentImageUsageRepository
    {
        /// <summary>
        /// İlgili tablolardaki görsel URL ve zengin metin alanlarının boş olmayan tüm değerlerini tek liste olarak döndürür.
        /// </summary>
        /// <returns>Tüm referans metinleri ve görsel URL listesi.</returns>
        Task<List<string>> GetReferenceTextsAsync();
    }
}
