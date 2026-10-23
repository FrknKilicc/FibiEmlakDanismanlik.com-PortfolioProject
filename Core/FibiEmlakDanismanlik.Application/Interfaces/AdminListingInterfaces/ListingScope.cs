using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces
{
    /// <summary>
    /// Kullanıcının ilan listeleme ve özet sorgularında görebileceği ilanların kapsam tanımı.
    /// </summary>
    public class ListingScope
    {
        /// <summary>
        /// Kullanıcının tüm ilanları görme yetkisi olup olmadığı (Owner, Manager veya CanViewAllListings olan danışmanlar).
        /// </summary>
        public bool SeeAll { get; set; }

        /// <summary>
        /// Danışmanın kendi ilanlarını filtrelemek için kullanılan AgentId değeri.
        /// </summary>
        public int? OwnAgentId { get; set; }

        /// <summary>
        /// Danışmana ListingAccessGrants tablosu üzerinden özel erişim izni verilmiş ilanların (ListingTypeId, ListingId) listesi.
        /// </summary>
        public IReadOnlyList<(int ListingTypeId, int ListingId)> GrantedListings { get; set; } = Array.Empty<(int, int)>();
    }
}
