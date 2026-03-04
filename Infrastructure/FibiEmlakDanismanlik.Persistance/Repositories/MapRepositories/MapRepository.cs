using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.MapRepositories
{
    public class MapRepository : IMapRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public MapRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<ListingMarkerResult> GetListingMarkerAsync(int listingId, int listingTypeId)
        {
            var lt = await _context.listingTypes.FirstOrDefaultAsync(x => x.ListingTypeId == listingTypeId);
            if (lt == null) return null;

            var geo = await _context.ListingGeoPoints
                .FirstOrDefaultAsync(x => x.ListingId == listingId && x.ListingTypeId == listingTypeId);

            if (geo == null) return null;

            var catKey = NormalizeCategoryKey(lt.Name);
            var usageKey = lt.UsageType == UsageType.ForSale ? "sale"
                        : lt.UsageType == UsageType.ForRent ? "rent"
                        : "both";

            string? status = null;
            string title = "";
            string? subtitle = null;

            if (lt.UsageType == UsageType.ForSale)
            {
                if (catKey == "housing")
                {
                    var item = await _context.forSaleHousingPropertyListings
                        .FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else if (catKey == "land")
                {
                    var item = await _context.forSaleLandListings
                        .FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else
                {
                    var item = await _context.forSaleCommercialPropertyListings
                        .FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
            }
            else if (lt.UsageType == UsageType.ForRent)
            {
                if (catKey == "housing")
                {
                    var item = await _context.rentalHousingListings
                        .FirstOrDefaultAsync(x => x.RentalHousingListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else if (catKey == "land")
                {
                    var item = await _context.rentalLandListings
                        .FirstOrDefaultAsync(x => x.RentalLandListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else
                {
                    var item = await _context.rentalCommercialPropertyListings
                        .FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
            }
            else
            {
                return null;
            }

            var statusKey = NormalizeStatusKey(status);
            var iconKey = $"{usageKey}_{catKey}_{statusKey}"; 

            return new ListingMarkerResult
            {
                ListingId = listingId,
                ListingTypeId = listingTypeId,
                Latitude = geo.Latitude,
                Longitude = geo.Longitude,
                Zoom = 15,
                Title = title,
                Subtitle = subtitle,
                PropertyStatus = status,
                IconKey = iconKey,
                UsageTypeId = (int)lt.UsageType,
                ListingTypeName = lt.Name
            };
        }

        private static string NormalizeCategoryKey(string? name)
        {
            var s = (name ?? "").Trim().ToLowerInvariant();
            if (s.Contains("konut")) return "housing";
            if (s.Contains("arsa")) return "land";
            return "commercial";
        }

        private static string NormalizeStatusKey(string? status)
        {
            var s = (status ?? "").Trim().ToLowerInvariant();
            if (s == "aktif" || s == "active" || s == "1" || s.Contains("aktif")) return "active";
            return "passive";
        }
    }
    }

