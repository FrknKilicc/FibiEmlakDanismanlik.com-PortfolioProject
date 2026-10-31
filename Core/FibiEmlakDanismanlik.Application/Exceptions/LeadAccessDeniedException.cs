using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    /// <summary>
    /// Kullanıcının talep ettiği müşteri talebi üzerinde görüntüleme veya işlem yetkisi bulunmadığında fırlatılan istisna.
    /// </summary>
    public class LeadAccessDeniedException : Exception
    {
        public LeadAccessDeniedException(string message = "Bu talebe erişim yetkiniz yok.") : base(message)
        {
        }
    }
}
