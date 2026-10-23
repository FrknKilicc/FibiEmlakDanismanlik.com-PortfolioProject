namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Prim / hak ediş payı ödeme durumları.
    /// </summary>
    public enum DealSharePaymentStatus
    {
        /// <summary>
        /// Ödeme bekliyor.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Personele ödendi.
        /// </summary>
        Paid = 2
    }
}
