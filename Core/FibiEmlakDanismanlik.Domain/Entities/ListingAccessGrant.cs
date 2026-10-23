using System;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Danışmana belirli bir ilan üzerinde verilen özel erişim/düzenleme yetkisi kaydı.
    /// </summary>
    public class ListingAccessGrant
    {
        [Key]
        public int ListingAccessGrantId { get; set; }

        /// <summary>
        /// Yetki verilen danışman kimliği.
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Yetkilendirilen ilan kimliği.
        /// </summary>
        public int ListingId { get; set; }

        /// <summary>
        /// İlan türü kimliği (satılık: 1, 2, 3; kiralık: 7, 8, 9).
        /// </summary>
        public int ListingTypeId { get; set; }

        /// <summary>
        /// Verilen erişim düzeyi (Görüntüleme veya Düzenleme).
        /// </summary>
        public ListingAccessLevel Access { get; set; }

        /// <summary>
        /// Yetkiyi veren danışman/yönetici kimliği.
        /// </summary>
        public int GrantedByAgentId { get; set; }

        /// <summary>
        /// Yetkinin verildiği UTC zamanı.
        /// </summary>
        public DateTime GrantedAtUtc { get; set; }
    }
}
