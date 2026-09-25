using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces
{
    public interface IListingTypeRepository
    {
        Task<List<ListingTypeResult>> GetByUsageTypeAsync(int usageType);
        Task<ListingTypeResult> GetByIdAsync(int id);
    }
}

