namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Benzer ilan bildirimi (haber bekleyenler) talep durumları.
    /// </summary>
    public enum ListingAlertStatus
    {
        /// <summary>
        /// Aktif talep; eşleşen ilanlar için bildirim gönderilebilir.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Kapandı; arayış tamamlandı veya talep iptal edildi.
        /// </summary>
        Closed = 2
    }
}
