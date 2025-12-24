using FibiEmlakDanismanlik.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public class PropertyStatusHelper
    {
        private static readonly HashSet<string> Allowed = new()
        {
            PropertyStatuses.Aktif,
            PropertyStatuses.Opsiyonlu,
            PropertyStatuses.Pasif,
            PropertyStatuses.Satildi,
            PropertyStatuses.Kiralandi
        };

        public static string NormalizeAndValidate(string? input)
        {
            var status = (input ?? "").Trim();

            if (string.IsNullOrWhiteSpace(status))
                return PropertyStatuses.Aktif;

            if (!Allowed.Contains(status))
                throw new ArgumentException($"Geçersiz İlan Statüsü : {status}");

            return status;
        }
    }
}
