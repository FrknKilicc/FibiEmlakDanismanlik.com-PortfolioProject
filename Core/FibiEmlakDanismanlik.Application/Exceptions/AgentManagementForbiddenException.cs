using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    public class AgentManagementForbiddenException : Exception
    {
        public AgentManagementForbiddenException(string message)
            : base(message)
        {
        }
    }
}
