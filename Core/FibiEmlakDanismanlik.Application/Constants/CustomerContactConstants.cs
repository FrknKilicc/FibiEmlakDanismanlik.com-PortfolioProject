namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// WebUI tüm ziyaretçiler adına tek IP'den geldiği için bu sınır yalnızca kaba bir üst sınırdır;
    /// asıl ziyaretçi bazlı sınır WebUI'deki FormSpamGuard'da uygulanır.
    /// </summary>
    public static class CustomerContactConstants
    {
        public const string RateLimitPolicy = "customer-contact";
        public const int RateLimitPermitCount = 30;
        public const int RateLimitWindowMinutes = 1;
    }
}
