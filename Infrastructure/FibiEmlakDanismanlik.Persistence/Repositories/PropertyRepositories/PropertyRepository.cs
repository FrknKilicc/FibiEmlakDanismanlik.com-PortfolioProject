using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.DTOs;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;

using FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories
{
    public partial class PropertyRepository : IPropertyRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public PropertyRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        // İlan fotoğraflarını ListingImages'tan okuyup PropImgUrl1..30 alanlarına yazar (eski kolon sırası)
        private async Task FillPhotoSlotsAsync<T>(IReadOnlyCollection<T> items, Func<T, (int ListingTypeId, int ListingId)> key) where T : class
        {
            if (items.Count == 0) return;
            var map = await ListingPhotoLookup.LoadAsync(_context, items.Select(key));
            foreach (var item in items)
                ListingImageSlots.Apply(item, map.TryGetValue(key(item), out var urls) ? urls : null);
        }

        private void FillPhotoSlots<T>(IReadOnlyCollection<T> items, Func<T, (int ListingTypeId, int ListingId)> key) where T : class
        {
            if (items.Count == 0) return;
            var map = ListingPhotoLookup.Load(_context, items.Select(key));
            foreach (var item in items)
                ListingImageSlots.Apply(item, map.TryGetValue(key(item), out var urls) ? urls : null);
        }

        // Kiralık konut/arsa/işyeri sorgularında ortak filtreler: tür, il/ilçe/mahalle, fiyat, danışman ve arama metni
        private static IQueryable<T> ApplyCommonRentalFilters<T>(IQueryable<T> query, PropertyFilterRequest filter) where T : RentalListingBaseResult
        {
            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                query = query.Where(x => filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (filter.CityId.HasValue)
                query = query.Where(x => x.CityId == filter.CityId);
            else if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                query = query.Where(x => x.DistrictId == filter.DistrictId);
            else if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                query = query.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            if (filter.AgentId.HasValue)
                query = query.Where(x => x.AgentId == filter.AgentId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                var dashIdx = term.IndexOf(" - ", StringComparison.Ordinal);
                var namePart = dashIdx >= 0 ? term.Substring(dashIdx + 3).Trim() : term;
                query = query.Where(x =>
                    (x.PropertyName != null && (x.PropertyName.Contains(term) || (namePart.Length > 0 && x.PropertyName.Contains(namePart))))
                    || (x.PropertyNo != null && x.PropertyNo.ToString() == term)
                    || (x.City != null && x.City.Contains(term))
                    || (x.District != null && x.District.Contains(term))
                    || (x.Neighborhood != null && x.Neighborhood.Contains(term)));
            }

            return query;
        }

        // İlan detayında özellikler: yalnızca aktif tanımlar, grup ve sıra numarasına göre
        private static Task<List<AmenityItemViewModel>> LoadAmenitiesAsync(IQueryable<AmenityDefinition> definitions)
        {
            return definitions
                .Where(d => d.isActive == true)
                .Select(d => new AmenityItemViewModel
                {
                    Id = d.AmenityId,
                    Text = d.Name,
                    Group = d.GroupName,
                    SortOrder = d.SortOrder
                })
                .OrderBy(x => x.Group)
                .ThenBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task<List<ListingSelectedImageRowDto>> GetSelectedImagesAsync(int listingId, string sectionKey)
        {
            return await _context.ListingImageSelections
        .Where(x => x.ListingId == listingId && x.SectionKey == sectionKey)
        .OrderBy(x => x.SortOrder)
        .Select(x => new ListingSelectedImageRowDto
        {
            ImageNo = x.ImageNo,
            SlotKey = x.SlotKey,
            SortOrder = x.SortOrder
        })
        .ToListAsync();
        }
    }
}
