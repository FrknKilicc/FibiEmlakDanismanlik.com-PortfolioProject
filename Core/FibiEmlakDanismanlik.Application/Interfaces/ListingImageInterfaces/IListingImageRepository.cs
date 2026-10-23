using FibiEmlakDanismanlik.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces
{
    /// <summary>
    /// İlan fotoğrafları için veri erişim ve eski kolonlarla çift yazma senkronizasyon sözleşmesi.
    /// </summary>
    public interface IListingImageRepository
    {
        /// <summary>
        /// Belirtilen ilan türü ve kimliğine sahip ilanın veritabanında mevcut olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="listingTypeId">İlan türü kimliği (ListingTypeId).</param>
        /// <param name="listingId">İlanın birincil anahtar kimliği.</param>
        /// <returns>İlan mevcutsa true, aksi halde false.</returns>
        Task<bool> ListingExistsAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Belirtilen ilana ait tüm görselleri SortOrder sırasına göre izlemesiz (AsNoTracking) olarak getirir.
        /// </summary>
        /// <param name="listingTypeId">İlan türü kimliği.</param>
        /// <param name="listingId">İlan kimliği.</param>
        /// <returns>SortOrder ile sıralanmış görsel listesi.</returns>
        Task<List<ListingImage>> GetByListingAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Belirtilen görsel kimliğine sahip görseli değişiklik takibi açık (tracked) olarak getirir.
        /// </summary>
        /// <param name="listingImageId">Görsel kimliği.</param>
        /// <returns>Görsel kaydı veya bulunamazsa null.</returns>
        Task<ListingImage?> GetByIdAsync(int listingImageId);

        /// <summary>
        /// Yeni görselleri veritabanına ekler ve değişiklikleri kaydeder.
        /// </summary>
        /// <param name="images">Eklenecek görsel koleksiyonu.</param>
        Task AddRangeAsync(IEnumerable<ListingImage> images);

        /// <summary>
        /// Belirtilen görseli veritabanından siler ve değişiklikleri kaydeder.
        /// </summary>
        /// <param name="image">Silinecek görsel varlığı.</param>
        Task RemoveAsync(ListingImage image);

        /// <summary>
        /// Verilen sırayla SortOrder'ı 1'den başlatır, IsCover yalnızca coverImageId'de true yapılır;
        /// listede olmayan görseller mevcut sıralamalarıyla sona eklenir ve kaydedilir.
        /// </summary>
        /// <param name="listingTypeId">İlan türü kimliği.</param>
        /// <param name="listingId">İlan kimliği.</param>
        /// <param name="orderedImageIds">Sıralanacak görsel kimlikleri listesi.</param>
        /// <param name="coverImageId">Kapak olarak belirlenecek görsel kimliği.</param>
        Task SaveOrderAsync(int listingTypeId, int listingId, IReadOnlyList<int> orderedImageIds, int coverImageId);

        /// <summary>
        /// İlanın tüm görsellerini SortOrder → ListingImageId sırasıyla yeniden numaralandırır (1'den kesintisiz)
        /// ve tam olarak bir kapak görseli olmasını garanti eder. Eşzamanlı yüklemelerin yarattığı
        /// tekrarlı SortOrder / çoklu IsCover tutarsızlıklarını düzeltir.
        /// Görsel yoksa hiçbir şey yapmaz.
        /// </summary>
        /// <param name="listingTypeId">İlan türü kimliği.</param>
        /// <param name="listingId">İlan kimliği.</param>
        Task NormalizeAsync(int listingTypeId, int listingId);

        /// <summary>
        /// O ilanın görsellerini (kapak önce, sonra SortOrder) okur ve ilgili tablonun PropImgUrl1..PropImgUrl30
        /// kolonlarına Url değerlerini yazar; kalan kolonları NULL yapar.
        /// </summary>
        /// <param name="listingTypeId">İlan türü kimliği.</param>
        /// <param name="listingId">İlan kimliği.</param>
        Task SyncLegacyColumnsAsync(int listingTypeId, int listingId);
    }
}
