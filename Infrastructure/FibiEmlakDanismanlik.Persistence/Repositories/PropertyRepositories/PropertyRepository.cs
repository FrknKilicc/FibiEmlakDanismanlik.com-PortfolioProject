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
using FibiEmlakDanismanlik.Persistence.Migrations;
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

        private static List<string> CollectImages(params string?[] urls)
        {
            return urls.Where(u => !string.IsNullOrWhiteSpace(u)).ToList()!;
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
