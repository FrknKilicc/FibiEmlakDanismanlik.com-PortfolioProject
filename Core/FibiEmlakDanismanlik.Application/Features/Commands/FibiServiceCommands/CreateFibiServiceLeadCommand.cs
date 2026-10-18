using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Application.Features.Commands.FibiServiceCommands
{
    /// <summary>FİBİ Hizmetler ortak danışmanlık/iletişim talebi oluşturma komutu.</summary>
    public class CreateFibiServiceLeadCommand : IRequest
    {
        [EnumDataType(typeof(FibiServiceModule), ErrorMessage = "Geçerli bir hizmet modülü seçiniz.")]
        public FibiServiceModule SourceModule { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [StringLength(FibiServiceConstants.FullNameMaxLength, MinimumLength = 3, ErrorMessage = "Ad soyad en az 3, en fazla 150 karakter olmalıdır.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Telefon zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(FibiServiceConstants.PhoneNumberMaxLength, MinimumLength = 10, ErrorMessage = "Telefon numarası en az 10, en fazla 20 karakter olmalıdır.")]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(FibiServiceConstants.EmailMaxLength, ErrorMessage = "E-posta en fazla 256 karakter olabilir.")]
        public string? Email { get; set; }

        public int? NeighborhoodId { get; set; }

        [StringLength(FibiServiceConstants.CalculationSummaryMaxLength, ErrorMessage = "Hesaplama özeti çok uzun.")]
        public string? CalculationSummaryJson { get; set; }

        [StringLength(FibiServiceConstants.CustomerNoteMaxLength, ErrorMessage = "Not en fazla 1000 karakter olabilir.")]
        public string? CustomerNote { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Kişisel verilerin işlenmesine onay vermelisiniz.")]
        public bool KvkkConsent { get; set; }
    }
}
