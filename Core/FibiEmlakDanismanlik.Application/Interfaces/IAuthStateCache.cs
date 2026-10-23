namespace FibiEmlakDanismanlik.Application.Interfaces
{
    /// <summary>
    /// Personel oturum durumlarının tutulduğu önbelleğin temizlenmesini sağlayan servis arayüzü.
    /// </summary>
    public interface IAuthStateCache
    {
        void Invalidate(int agentId);
    }
}
