namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Müşteri talebinin geldiği kaynak.
    /// </summary>
    public enum LeadSource
    {
        /// <summary>
        /// Web sitesi iletişim veya ilan randevu formu.
        /// </summary>
        Contact = 1,

        /// <summary>
        /// FİBİ hizmetleri analiz ve danışmanlık talep formu.
        /// </summary>
        FibiService = 2
    }
}
