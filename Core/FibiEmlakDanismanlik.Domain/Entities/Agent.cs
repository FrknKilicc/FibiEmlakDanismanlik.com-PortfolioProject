using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Agent
    {
        [Key]
        public int AgentId { get; set; }

        [Required(ErrorMessage = "Personel Adı Giriniz")]
        public string AgentName { get; set; }

        [Display(Name = "Ünvan")]
        public string AgentTitle { get; set; }

        [Display(Name = "Açıklama")]
        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string AgentDescription { get; set; }

        [Display(Name = "Resim URL")]
        [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
        public string AgentImgUrl { get; set; }

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta adresi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Mail { get; set; }

        [Display(Name = "Telefon Numarası")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string AgentPhoneNumber { get; set; }

        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }

    }
}
