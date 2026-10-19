using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using FibiEmlakDanismanlik.Application.Models.MapModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces
{
    public interface IMapRepository
    {
        public Task<ListingMarkerResult> GetListingMarkerAsync(int listingId, int listingTypeId);
        Task<List<GetMapListingsResult>> GetMapListingsAsync(int? usageType, string? category, int? cityId);
        Task<List<ListingGeocodeCandidate>> GetListingsWithoutGeoPointAsync(int take);
        Task<int> AddGeoPointsAsync(IReadOnlyList<ListingGeoPoint> points);
    }
}
