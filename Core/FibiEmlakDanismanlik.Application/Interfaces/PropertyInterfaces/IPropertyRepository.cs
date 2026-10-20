using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
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

        public Task<ForSalePropertyForListingViewModel> GetUnifiedForSalePropertyById(int id, int? listingTypeId = null);
        public List<ForRentalPropertForListingViewModel> GetAllForRentalPropertyForListing();
        Task<List<ListingTypeFacetDto>> GetForSaleListingTypeFacetsAsync();
        public Task<List<ForSalePropertyForListingViewModel>> GetAllForSalePropertyForListing();
        public Task<List<ForSalePropertyForListingViewModel>> GetFilteredForSalePropertyForListing(PropertyFilterRequest filter);
        public Task<ForSaleFilterResponseResult> GetFilteredForSalePropertyForListingWithFacets(PropertyFilterRequest filter);

        public Task<List<ListingTypeFacetDto>> GetForRentalListingTypeFacetsAsync();
        public Task<RentalListingFilterResponseResult> GetFilteredForRentalPropertyForListingWithFacets(PropertyFilterRequest filter);
        public Task<ForRentalPropertForListingViewModel> GetUnifiedForRentalPropertyById(int id, int? listingTypeId = null);

        public Task<List<ListingSelectedImageRowDto>>
            GetSelectedImagesAsync(int listingId, string sectionKey);

        public Task<SimilarPropertyReferenceResult?> GetForSaleSimilarReferenceAsync(int listingId);
        public Task<List<SimilarPropertyCandidateResult>> GetForSaleSimilarCandidatesAsync(SimilarPropertyReferenceResult reference, int take, int relaxLevel = 0);

        public Task<SimilarPropertyReferenceResult?> GetForRentalSimilarReferenceAsync(int listingId);
        public Task<List<SimilarPropertyCandidateResult>> GetForRentalSimilarCandidatesAsync(SimilarPropertyReferenceResult reference, int take, int relaxLevel = 0);

        public Task<List<FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.PropertySuggestionDto>> GetPropertySuggestionsAsync(int usageType, int? listingTypeId, string searchTerm);
        public Task<FibiEmlakDanismanlik.Application.Features.Results.PropertyResults.SearchFilterFacetsResult> GetSearchFilterFacetsAsync(int usageType, int listingTypeId, int? cityId, int? districtId);
    }
}
