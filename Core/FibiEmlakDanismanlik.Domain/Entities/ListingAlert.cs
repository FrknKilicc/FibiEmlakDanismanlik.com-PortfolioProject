using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Benzer ilan çıktığında haber verilmesi talebi (haber bekleyenler).
    /// </summary>
    public class ListingAlert
    {
        [Key]
        public int ListingAlertId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        /// <summary>
        /// İlan türü (1 satılık arsa, 2 satılık konut, 3 satılık işyeri, 7 kiralık arsa, 8 kiralık konut, 9 kiralık işyeri).
        /// </summary>
        public int ListingTypeId { get; set; }

        public int CityId { get; set; }

        public int? DistrictId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Virgülle ayrılmış oda sayısı tercihleri (örn: "2+1,3+1"); yalnızca konut türlerinde kullanılır.
        /// </summary>
        [MaxLength(100)]
        public string? RoomCounts { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        /// <summary>
        /// Formun gönderildiği kaynak ilanın türü.
        /// </summary>
        public int? SourceListingTypeId { get; set; }

        /// <summary>
        /// Formun gönderildiği kaynak ilanın kimliği.
        /// </summary>
        public int? SourceListingId { get; set; }

        /// <summary>
        /// Talep takip durumu.
        /// </summary>
        public ListingAlertStatus Status { get; set; } = ListingAlertStatus.Active;

        /// <summary>
        /// Talebe atanan danışman kimliği.
        /// </summary>
        public int? AssignedAgentId { get; set; }

        /// <summary>
        /// KVKK aydınlatma metninin onaylandığı tarih ve saat (UTC).
        /// </summary>
        public DateTime KvkkApprovedAt { get; set; }

        /// <summary>
        /// Talebin oluşturulma tarihi (yerel saat).
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Talebin kapatılma tarihi (UTC).
        /// </summary>
        public DateTime? ClosedAtUtc { get; set; }

        /// <summary>
        /// Bu talep ile eşleştirilen ilan bildirim kayıtları.
        /// </summary>
        public ICollection<ListingAlertMatch> Matches { get; set; } = new List<ListingAlertMatch>();
    }
}
