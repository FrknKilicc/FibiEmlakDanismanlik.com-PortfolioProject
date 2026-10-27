using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public  class CustomerContact
    {
        public int CustomerContactId { get; set; }

        public string CustomerContactName { get; set; }
        public string? CustomerContactMail { get; set; }
        public string CustomerContactPhone { get; set; }

        public string CustomerContactMessage { get; set; }

        public DateTime CreatedDate { get; set; } = TurkeyClock.Now;

        
        public int? PropertyListingId { get; set; }

        public PropertyListingType? PropertyListingTypeEnum { get; set; }

        // Randevu tarihi
        public DateTime? AppointmentDateTime { get; set; }

        /// <summary>
        /// Talep takip durumu.
        /// </summary>
        public LeadStatus LeadStatus { get; set; } = LeadStatus.New;

        /// <summary>
        /// Talebe atanan danışman kimliği.
        /// </summary>
        public int? AssignedAgentId { get; set; }

        /// <summary>
        /// Talep durumunun son güncellenme tarihi (UTC).
        /// </summary>
        public DateTime? StatusUpdatedAtUtc { get; set; }

        /// <summary>
        /// KVKK aydınlatma metni onay zamanı (UTC). Bu alan eklenmeden önceki kayıtlarda boştur.
        /// </summary>
        public DateTime? KvkkApprovedAt { get; set; }
    }
}
