using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace FibiEmlakDanismanlik.Infrastructure.Security
{
    /// <summary>
    /// TOTP (Time-based One-time Password) üretimi, doğrulaması ve anahtar şifreleme servisi.
    /// </summary>
    public class TwoFactorService : ITwoFactorService
    {
        private static readonly object _lock = new();
        private readonly IDataProtector _protector;
        private readonly IMemoryCache _memoryCache;

        public TwoFactorService(IDataProtectionProvider dataProtectionProvider, IMemoryCache memoryCache)
        {
            _protector = dataProtectionProvider.CreateProtector(AuthConstants.TwoFactorProtectorPurpose);
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 20 baytlık rastgele anahtar üreterek Base32 formatında döndürür.
        /// </summary>
        public string GenerateSecret()
        {
            var keyBytes = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(keyBytes);
        }

        /// <summary>
        /// Authenticator uygulamalarının tanıması için standart otpauth:// URI'si oluşturur.
        /// </summary>
        public string BuildOtpAuthUri(string accountMail, string secret)
        {
            var issuerEscaped = Uri.EscapeDataString(AuthConstants.TotpIssuer);
            var mailEscaped = Uri.EscapeDataString(accountMail ?? string.Empty);
            return $"otpauth://totp/{issuerEscaped}:{mailEscaped}?secret={secret}&issuer={issuerEscaped}&algorithm=SHA1&digits=6&period=30";
        }

        /// <summary>
        /// Kullanıcının girdiği kodu SHA1, 30 saniye periyot ve ±1 pencere (window) ile doğrular.
        /// </summary>
        public bool VerifyCode(string secret, string code)
        {
            return TryVerifyTotp(secret, code, out _);
        }

        /// <summary>
        /// Kodu doğrular ve aynı hesap için daha önce kabul edilmiş (aynı veya daha eski) zaman adımındaki bir kodu reddeder (tekrar kullanım koruması).
        /// Koruma sunucu belleğindedir (IMemoryCache) ve tek örnekli (tek sunucu) kurulum içindir.
        /// </summary>
        public bool VerifyCodeOnce(int agentId, string secret, string code)
        {
            if (!TryVerifyTotp(secret, code, out long timeStepMatched))
                return false;

            var cacheKey = $"totp-last-step:{agentId}";

            lock (_lock)
            {
                if (_memoryCache.TryGetValue(cacheKey, out long lastStep) && timeStepMatched <= lastStep)
                {
                    return false;
                }

                _memoryCache.Set(cacheKey, timeStepMatched, TimeSpan.FromMinutes(5));
                return true;
            }
        }

        private static bool TryVerifyTotp(string secret, string code, out long timeStepMatched)
        {
            timeStepMatched = 0;

            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
                return false;

            var cleanCode = code.Replace(" ", "").Trim();
            if (cleanCode.Length != 6 || !cleanCode.All(char.IsDigit))
                return false;

            byte[] secretBytes;
            try
            {
                secretBytes = Base32Encoding.ToBytes(secret);
            }
            catch
            {
                return false;
            }

            var totp = new Totp(secretBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(cleanCode, out timeStepMatched, new VerificationWindow(previous: 1, future: 1));
        }

        /// <summary>
        /// TOTP gizli anahtarını Data Protection ile şifreler.
        /// </summary>
        public string Protect(string secret)
        {
            if (secret == null)
                throw new ArgumentNullException(nameof(secret));

            return _protector.Protect(secret);
        }

        /// <summary>
        /// Şifrelenmiş TOTP gizli anahtarını çözer; çözülemez veya bozuksa null döndürür.
        /// </summary>
        public string? Unprotect(string? protectedSecret)
        {
            if (string.IsNullOrWhiteSpace(protectedSecret))
                return null;

            try
            {
                return _protector.Unprotect(protectedSecret);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}
