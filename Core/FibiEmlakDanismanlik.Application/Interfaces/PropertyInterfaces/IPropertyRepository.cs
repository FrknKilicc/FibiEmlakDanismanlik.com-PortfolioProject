using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.DTOs;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces
{
    public interface IPropertyRepository
    {
        public List<ForSalePropertyViewModel> GetAllForSalePropertyWithAgent();
        public List<RentalPropertyBaseViewModel> GetAllRentalPropertyWithAgent();

        public Task<ForSalePropertyForListingViewModel> GetUnifiedForSalePropertyById(int id);
        public List<ForRentalPropertForListingViewModel> GetAllForRentalPropertyForListing();
        Task<List<ListingTypeFacetDto>> GetForSaleListingTypeFacetsAsync();
        public Task<List<ForSalePropertyForListingViewModel>> GetAllForSalePropertyForListing();
        public Task<List<ForSalePropertyForListingViewModel>> GetFilteredForSalePropertyForListing(PropertyFilterRequest filter);
        public Task<ForSaleFilterResponseResult> GetFilteredForSalePropertyForListingWithFacets(PropertyFilterRequest filter);
    }
}
