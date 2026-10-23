using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Danışman ve kullanıcı hesaplarının panel rolü.
    /// </summary>
    public enum AgentRole
    {
        /// <summary>
        /// Sahip: Tüm sistem yetkilerine sahip en üst düzey hesap.
        /// </summary>
        Owner = 1,

        /// <summary>
        /// Yönetici: Danışmanları ve ilanları yönetme yetkisine sahip hesap.
        /// </summary>
        Manager = 2,

        /// <summary>
        /// Danışman: İlan oluşturabilen ve kendisine atanan ilanları yönetebilen hesap.
        /// </summary>
        Consultant = 3
    }
}
