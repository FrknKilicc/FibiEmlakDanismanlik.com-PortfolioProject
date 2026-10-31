using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands
{
    /// <summary>
    /// Siteden "Benzer ilan çıkınca haber ver" talebi oluşturma komutu.
    /// </summary>
    public class CreateListingAlertCommand : IRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ListingTypeId { get; set; }
        public int CityId { get; set; }
        public int? DistrictId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? RoomCounts { get; set; }
        public string? Note { get; set; }
        public int? SourceListingTypeId { get; set; }
        public int? SourceListingId { get; set; }
        public bool KvkkApproved { get; set; }
    }
}
