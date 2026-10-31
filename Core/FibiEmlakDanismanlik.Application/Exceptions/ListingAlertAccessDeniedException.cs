using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    /// <summary>
    /// Kullanıcının haber bekleyen talep kaydı üzerinde görüntüleme veya işlem yetkisi bulunmadığında fırlatılan istisna.
    /// </summary>
    public class ListingAlertAccessDeniedException : Exception
    {
        public ListingAlertAccessDeniedException(string message = "Bu haber talebine erişim yetkiniz yok.") : base(message)
        {
        }
    }
}
