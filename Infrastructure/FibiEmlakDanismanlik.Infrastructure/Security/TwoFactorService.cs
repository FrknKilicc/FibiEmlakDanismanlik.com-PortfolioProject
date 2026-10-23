using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector _protector;

        public TwoFactorService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector(AuthConstants.TwoFactorProtectorPurpose);
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
            return totp.VerifyTotp(cleanCode, out _, new VerificationWindow(previous: 1, future: 1));
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
