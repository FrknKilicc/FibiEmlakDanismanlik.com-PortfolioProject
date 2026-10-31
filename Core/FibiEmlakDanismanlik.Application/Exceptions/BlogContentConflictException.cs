using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    public class BlogContentConflictException : Exception
    {
        public BlogContentConflictException(string message)
            : base(message)
        {
        }
    }
}
