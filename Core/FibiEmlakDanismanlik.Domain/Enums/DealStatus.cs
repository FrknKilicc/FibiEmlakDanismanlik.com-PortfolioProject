namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Satış ve kiralama işlem takip durumları.
    /// </summary>
    public enum DealStatus
    {
        /// <summary>
        /// Kapora alındı, işlem devam ediyor.
        /// </summary>
        DepositReceived = 1,

        /// <summary>
        /// İşlem başarıyla tamamlandı (satış/kiralama gerçekleşti).
        /// </summary>
        Completed = 2,

        /// <summary>
        /// İşlem iptal edildi.
        /// </summary>
        Cancelled = 3
    }
}
