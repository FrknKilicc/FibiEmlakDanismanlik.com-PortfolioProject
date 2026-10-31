using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FibiEmlakDanismanlik.WebApi.Services
{
    /// <summary>
    /// Personel oturum durumu önbelleğini temizleyen servis implementasyonu.
    /// </summary>
    public class AuthStateCache : IAuthStateCache
    {
        private readonly IMemoryCache _memoryCache;

        public AuthStateCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Invalidate(int agentId)
        {
            _memoryCache.Remove(AuthConstants.AuthStateCacheKey(agentId));
        }
    }
}
