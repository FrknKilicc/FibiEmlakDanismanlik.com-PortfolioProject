namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// İlan bildirim eşleşme durumları.
    /// </summary>
    public enum ListingAlertMatchStatus
    {
        /// <summary>
        /// Müşteriye haber verildi / bildirim gönderildi.
        /// </summary>
        Notified = 1,

        /// <summary>
        /// İlan talep için uygun değil olarak işaretlendi / elendi.
        /// </summary>
        Dismissed = 2
    }
}
