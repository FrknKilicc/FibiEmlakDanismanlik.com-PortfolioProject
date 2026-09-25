using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
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

        public async Task<List<ListingTypeResult>> GetByUsageTypeAsync(int usageType)
        {
            return await _context.listingTypes
                .AsNoTracking()
                .Where(x => (int)x.UsageType == usageType || (int)x.UsageType == 3)
                .OrderBy(x => x.Name)
                .Select(x => new ListingTypeResult(x.ListingTypeId, x.Name))
                .ToListAsync();
        }

        public async Task<ListingTypeResult> GetByIdAsync(int id)
        {
            var entity = await _context.listingTypes.FindAsync(id);
            if (entity == null) return null;
            return new ListingTypeResult(entity.ListingTypeId, entity.Name);
        }
    }
}

