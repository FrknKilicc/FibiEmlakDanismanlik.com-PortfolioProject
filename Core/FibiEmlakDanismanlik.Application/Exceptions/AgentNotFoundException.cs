using System;

namespace FibiEmlakDanismanlik.Application.Exceptions
{
    public class AgentNotFoundException : Exception
    {
        public AgentNotFoundException(string message = "Personel bulunamadı.")
            : base(message)
        {
        }

        public AgentNotFoundException(int agentId)
            : base($"'{agentId}' kimlikli personel bulunamadı.")
        {
        }
    }
}
