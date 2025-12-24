using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Constants
{
    public class PropertyStatuses
    {
        
        public const string Aktif = "Aktif";
        public const string Opsiyonlu = "Opsiyonlu";
        public const string Pasif = "Pasif";

        
        public const string Satildi = "Satıldı";
        public const string Kiralandi = "Kiralandı";

        public static readonly string[] ActiveSet = { Aktif, Opsiyonlu };
    }
}
