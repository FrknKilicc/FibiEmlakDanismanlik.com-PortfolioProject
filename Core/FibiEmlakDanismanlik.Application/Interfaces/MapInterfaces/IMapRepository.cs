using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
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
    }
}
