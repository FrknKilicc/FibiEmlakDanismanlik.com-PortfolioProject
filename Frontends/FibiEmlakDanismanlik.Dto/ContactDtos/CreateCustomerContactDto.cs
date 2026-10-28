using System;

namespace FibiEmlakDanismanlik.Dto.ContactDtos
{
    public class CreateCustomerContactDto
    {
        public string CustomerContactName { get; set; } = string.Empty;
        public string? CustomerContactMail { get; set; }
        public string CustomerContactPhone { get; set; } = string.Empty;
        public string? CustomerContactMessage { get; set; }
        public int? PropertyListingTypeEnum { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public int? PropertyListingId { get; set; }
        public bool KvkkApproved { get; set; }
    }
}
