using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries
{
    /// <summary>Bina risk analizi ve kentsel dönüşüm puanı hesaplama sorgusu.</summary>
    public class CalculateBuildingRiskQuery : IRequest<CalculateBuildingRiskResult?>, IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir mahalle seçiniz.")]
        public int NeighborhoodId { get; set; }

        [EnumDataType(typeof(ConstructionYearRange), ErrorMessage = "Geçerli bir yapım yılı aralığı seçiniz.")]
        public ConstructionYearRange ConstructionYearRange { get; set; }

        [Range(1, 60, ErrorMessage = "Toplam kat sayısı 1 ile 60 arasında olmalıdır.")]
        public int TotalFloors { get; set; }

        [Range(-3, 60, ErrorMessage = "Bulunduğunuz kat -3 ile 60 arasında olmalıdır.")]
        public int CurrentFloor { get; set; }

        [EnumDataType(typeof(StructureType), ErrorMessage = "Geçerli bir yapı tipi seçiniz.")]
        public StructureType StructureType { get; set; }

        public bool HasGroundFloorCommercial { get; set; }
        public bool IsAdjacentBuilding { get; set; }
        public bool HasBasementOrParking { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentFloor > TotalFloors)
            {
                yield return new ValidationResult(
                    "Bulunduğunuz kat, toplam kat sayısından büyük olamaz.",
                    new[] { nameof(CurrentFloor) });
            }
        }
    }
}
