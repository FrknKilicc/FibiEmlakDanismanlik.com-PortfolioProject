using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingTypeRepositories
{
    public class ListingTypeRepository : IListingTypeRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ListingTypeRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<List<ListingTypeResult>> GetByUsageTypeAsync(int usageType, bool onlyWithActiveListings = false)
        {
            var query = _context.listingTypes
                .AsNoTracking()
                .Where(x => (int)x.UsageType == usageType || (int)x.UsageType == 3);

            if (onlyWithActiveListings)
            {
                var activeStatuses = PropertyStatuses.ActiveSet;
                if (usageType == (int)UsageType.ForSale)
                {
                    query = query.Where(lt =>
                        lt.UsageType == UsageType.ForSale && (
                            _context.forSaleHousingPropertyListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus)) ||
                            _context.forSaleLandListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus)) ||
                            _context.forSaleCommercialPropertyListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus))
                        ));
                }
                else if (usageType == (int)UsageType.ForRent)
                {
                    query = query.Where(lt =>
                        lt.UsageType == UsageType.ForRent && (
                            _context.rentalHousingListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus)) ||
                            _context.rentalLandListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus)) ||
                            _context.rentalCommercialPropertyListings.Any(p => p.ListingTypeId == lt.ListingTypeId && p.PropertyStatus != null && activeStatuses.Contains(p.PropertyStatus))
                        ));
                }
                else
                {
                    return new List<ListingTypeResult>();
                }
            }

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new ListingTypeResult(x.ListingTypeId, x.Name, (int)x.UsageType))
                .ToListAsync();
        }

        public async Task<ListingTypeResult> GetByIdAsync(int id)
        {
            var entity = await _context.listingTypes.FindAsync(id);
            if (entity == null) return null;
            return new ListingTypeResult(entity.ListingTypeId, entity.Name, (int)entity.UsageType);
        }
    }
}

