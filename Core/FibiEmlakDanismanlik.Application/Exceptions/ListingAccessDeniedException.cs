using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    /// <summary>
    /// Kullanıcının talep ettiği ilan üzerinde görüntüleme veya işlem yetkisi bulunmadığında fırlatılan istisna.
    /// </summary>
    public class ListingAccessDeniedException : Exception
    {
        public ListingAccessDeniedException(string message = "Bu ilanı görüntüleme yetkiniz yok.") : base(message)
        {
        }
    }
}
