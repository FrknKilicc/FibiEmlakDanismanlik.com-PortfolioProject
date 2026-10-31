using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    /// <summary>
    /// Kullanıcının işlem ve hak ediş (prim) bilgileri üzerinde görüntüleme veya işlem yetkisi bulunmadığında fırlatılan istisna.
    /// </summary>
    public class DealAccessDeniedException : Exception
    {
        public DealAccessDeniedException(string message = "İşlem ve hak ediş bilgilerine erişim yetkiniz yok.") : base(message)
        {
        }
    }
}
