using System;
using System.ComponentModel.DataAnnotations;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Müşteri taleplerine ait hareket, not ve durum geçmişi kaydı.
    /// </summary>
    public class LeadActivity
    {
        [Key]
        public int LeadActivityId { get; set; }

        /// <summary>
        /// Talebin kaynak tablosu (iletişim/randevu formu veya FİBİ hizmetleri).
        /// </summary>
        public LeadSource Source { get; set; }

        /// <summary>
        /// İlgili kaynak tablosundaki talep kimliği (CustomerContactId veya FibiServiceLeadId).
        /// </summary>
        public int LeadId { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren danışman kimliği (sistem/otomatik işlemlerde null olabilir).
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// Hareket türü (Not, Durum Değişikliği, Atama, Arama).
        /// </summary>
        public LeadActivityType Type { get; set; }

        /// <summary>
        /// Hareket açıklaması veya not içeriği (en fazla 2000 karakter).
        /// </summary>
        [MaxLength(2000)]
        public string? Text { get; set; }

        /// <summary>
        /// Durum değişikliği öncesindeki eski durum.
        /// </summary>
        public LeadStatus? OldStatus { get; set; }

        /// <summary>
        /// Durum değişikliği sonrasındaki yeni durum.
        /// </summary>
        public LeadStatus? NewStatus { get; set; }

        /// <summary>
        /// Atama hareketinde yeni atanan danışman kimliği.
        /// </summary>
        public int? AssignedAgentId { get; set; }

        /// <summary>
        /// Hareketin oluşturulma tarihi (UTC).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
