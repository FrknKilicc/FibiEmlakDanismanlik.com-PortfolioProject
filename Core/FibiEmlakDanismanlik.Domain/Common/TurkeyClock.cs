namespace FibiEmlakDanismanlik.Domain.Common
{
    // shortcut: Türkiye UTC+3 sabit (2016'dan beri yaz saati yok); kural değişirse TimeZoneInfo ile hesaplanmalı
    public static class TurkeyClock
    {
        public static DateTime Now => DateTime.UtcNow.AddHours(3);
        public static DateTime Today => Now.Date;
    }
}
