namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Talep hareket ve işlem türleri.
    /// </summary>
    public enum LeadActivityType
    {
        /// <summary>
        /// Danışman notu.
        /// </summary>
        Note = 1,

        /// <summary>
        /// Durum değişikliği.
        /// </summary>
        StatusChange = 2,

        /// <summary>
        /// Danışman ataması veya devri.
        /// </summary>
        Assignment = 3,

        /// <summary>
        /// Telefon araması veya görüşme kaydı.
        /// </summary>
        Call = 4
    }
}
