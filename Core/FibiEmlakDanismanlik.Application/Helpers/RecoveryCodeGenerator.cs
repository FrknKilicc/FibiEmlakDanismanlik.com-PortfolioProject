using System.Collections.Generic;
using System.Security.Cryptography;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>
    /// İki adımlı doğrulama için kriptografik olarak güvenli kurtarma kodları üretir.
    /// </summary>
    public static class RecoveryCodeGenerator
    {
        private const string RecoveryCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>
        /// Belirtilen sayıda XXXX-XXXX formatında kurtarma kodu üretir.
        /// </summary>
        public static List<string> Generate(int count)
        {
            var recoveryCodes = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                var chars = new char[8];
                for (int j = 0; j < 8; j++)
                {
                    chars[j] = RecoveryCodeAlphabet[RandomNumberGenerator.GetInt32(RecoveryCodeAlphabet.Length)];
                }
                var code = $"{new string(chars, 0, 4)}-{new string(chars, 4, 4)}";
                recoveryCodes.Add(code);
            }
            return recoveryCodes;
        }
    }
}
