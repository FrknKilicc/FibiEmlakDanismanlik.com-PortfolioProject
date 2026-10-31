using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    public class DuplicateAgentMailException : Exception
    {
        public DuplicateAgentMailException(string mail)
            : base("Bu e-posta adresi başka bir personel tarafından kullanılıyor.")
        {
        }
    }
}
