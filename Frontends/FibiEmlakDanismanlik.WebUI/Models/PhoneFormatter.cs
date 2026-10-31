using System;
using System.Linq;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public static class PhoneFormatter
    {
        public static (string Display, string Href) Format(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return (string.Empty, string.Empty);
            }

            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digits.Length == 11 && digits.StartsWith('0'))
            {
                // 0XXX XXX XX XX
                return ($"0{digits[1..4]} {digits[4..7]} {digits[7..9]} {digits[9..11]}", $"tel:+90{digits[1..]}");
            }
            else if (digits.Length == 10)
            {
                // Başına 0 ekleyerek aynı biçim
                return ($"0{digits[0..3]} {digits[3..6]} {digits[6..8]} {digits[8..10]}", $"tel:+90{digits}");
            }
            else
            {
                // Diğer durumlarda ham değer
                return (phoneNumber, $"tel:{digits}");
            }
        }

        /// <summary>
        /// Yalnızca Türkiye cep telefonları (5XX) için WhatsApp uyumlu "905XXXXXXXXX" formatında numara döndürür;
        /// sabit hat veya geçersiz numaralar için null döner.
        /// </summary>
        public static string? WhatsAppNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digits.Length == 11 && digits.StartsWith('0'))
            {
                digits = digits[1..];
            }
            else if (digits.Length == 12 && digits.StartsWith("90"))
            {
                digits = digits[2..];
            }

            if (digits.Length == 10 && digits.StartsWith('5'))
            {
                return $"90{digits}";
            }

            return null;
        }
    }
}
