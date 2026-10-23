namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// İptal durumunda kaporanın akıbeti.
    /// </summary>
    public enum DepositOutcome
    {
        /// <summary>
        /// İptalde kapora alıcıya/kiracıya geri verilmedi (varsayılan).
        /// </summary>
        Kept = 1,

        /// <summary>
        /// İptalde kapora iade edildi.
        /// </summary>
        Refunded = 2
    }
}
