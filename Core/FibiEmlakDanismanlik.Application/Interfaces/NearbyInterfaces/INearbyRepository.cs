using FibiEmlakDanismanlik.Application.Features.Results.NearbyResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.NearbyInterfaces
{
    public interface INearbyRepository
    {
        public Task<List<NearbyCategoryGroupResult>> GetNearbyByListingAsync(int listingId, int listingType);
    }
}
