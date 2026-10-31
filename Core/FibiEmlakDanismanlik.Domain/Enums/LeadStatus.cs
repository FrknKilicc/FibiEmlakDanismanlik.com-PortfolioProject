namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Müşteri talep durumları.
    /// </summary>
    public enum LeadStatus
    {
        /// <summary>
        /// Yeni talep.
        /// </summary>
        New = 1,

        /// <summary>
        /// Müşteri ile iletişime geçildi / arandı.
        /// </summary>
        Contacted = 2,

        /// <summary>
        /// Randevu planlandı.
        /// </summary>
        Appointment = 3,

        /// <summary>
        /// Teklif verildi / değerlendirmede.
        /// </summary>
        Offer = 4,

        /// <summary>
        /// Kapora alındı.
        /// </summary>
        Deposit = 5,

        /// <summary>
        /// Başarıyla sonuçlandı (satış/kiralama gerçekleşti).
        /// </summary>
        Won = 6,

        /// <summary>
        /// Olumsuz sonuçlandı / kaybedildi.
        /// </summary>
        Lost = 7
    }
}
