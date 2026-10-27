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
