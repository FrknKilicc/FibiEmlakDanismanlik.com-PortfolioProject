using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    public class DuplicateHomeSectionImageSlotException : Exception
    {
        public DuplicateHomeSectionImageSlotException(string sectionKey, int slot)
            : base($"{sectionKey} bölümünde {slot}. görsel zaten tanımlı.")
        {
        }
    }
}
