using System;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class ResultBestDealDto
    {
        public int ListingId { get; set; }
        public int UsageType { get; set; }
        public int? ListingTypeId { get; set; }
        public string ListingTypeName { get; set; } = "";
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public decimal Price { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? ImageUrl { get; set; }
        public double? Area { get; set; }
        public string? NumberOfRoom { get; set; }
        public int? NumberOfBathRoom { get; set; }
        public int? NumberOfSection { get; set; }
        public string? ZoningStatus { get; set; }
        public string? AgentName { get; set; }
        public string? AgentImgUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
