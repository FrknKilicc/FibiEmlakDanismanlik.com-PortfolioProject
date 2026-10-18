using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries
{
    /// <summary>Mahalle piyasa analizi ve opsiyonel karşılaştırma sorgusu.</summary>
    public class GetNeighborhoodAnalysisQuery : IRequest<GetNeighborhoodAnalysisResult?>, IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir mahalle seçiniz.")]
        public int NeighborhoodId { get; set; }

        public int? CompareWithNeighborhoodId { get; set; }

        [EnumDataType(typeof(PropertySegment), ErrorMessage = "Geçerli bir gayrimenkul segmenti seçiniz.")]
        public PropertySegment PropertySegment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CompareWithNeighborhoodId.HasValue && CompareWithNeighborhoodId.Value == NeighborhoodId)
            {
                yield return new ValidationResult(
                    "Karşılaştırma için farklı bir mahalle seçin.",
                    new[] { nameof(CompareWithNeighborhoodId) });
            }
        }
    }
}
