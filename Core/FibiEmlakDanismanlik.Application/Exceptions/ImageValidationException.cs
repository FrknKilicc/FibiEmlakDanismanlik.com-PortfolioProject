using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    /// <summary>
    /// Görsel doğrulama, boyutlandırma veya format hatalarında kullanıcıya gösterilebilir mesaj taşıyan özel istisna sınıfı.
    /// </summary>
    public class ImageValidationException : Exception
    {
        public ImageValidationException(string message) : base(message)
        {
        }

        public ImageValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
