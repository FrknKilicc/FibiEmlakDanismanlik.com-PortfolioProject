using FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingTypeHandlers
{
    public class GetAdvancedFiltersByListingTypeQueryHandler : IRequestHandler<GetAdvancedFiltersByListingTypeQuery, AdvancedFiltersVisibilityDto>
    {
        private readonly IPropertyRepository _propertyRepository;
        private static readonly StringComparer TrComparer = StringComparer.Create(new CultureInfo("tr-TR"), true);

        public GetAdvancedFiltersByListingTypeQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<AdvancedFiltersVisibilityDto> Handle(GetAdvancedFiltersByListingTypeQuery request, CancellationToken cancellationToken)
        {
            var listingTypeEnum = (PropertyListingType)request.ListingTypeId;
            string category;
            int expectedUsageType;

            switch (listingTypeEnum)
            {
                case PropertyListingType.ForSaleLandListing:
                    category = "land";
                    expectedUsageType = (int)UsageType.ForSale;
                    break;
                case PropertyListingType.ForSaleHousingListing:
                    category = "housing";
                    expectedUsageType = (int)UsageType.ForSale;
                    break;
                case PropertyListingType.ForSaleCommercialPropertyListing:
                    category = "commercial";
                    expectedUsageType = (int)UsageType.ForSale;
                    break;
                case PropertyListingType.RentalLandListing:
                    category = "land";
                    expectedUsageType = (int)UsageType.ForRent;
                    break;
                case PropertyListingType.RentalHousingListing:
                    category = "housing";
                    expectedUsageType = (int)UsageType.ForRent;
                    break;
                case PropertyListingType.RentalCommercialPropertyListing:
                    category = "commercial";
                    expectedUsageType = (int)UsageType.ForRent;
                    break;
                default:
                    return new AdvancedFiltersVisibilityDto
                    {
                        ShowDistance = false,
                        TotalCount = 0
                    };
            }

            var effectiveUsageType = request.UsageType > 0 ? request.UsageType : expectedUsageType;
            var isLand = category == "land";
            var isCommercial = category == "commercial";
            var isForSale = expectedUsageType == (int)UsageType.ForSale;

            var result = new AdvancedFiltersVisibilityDto
            {
                Category = category,
                RoomLabel = isCommercial ? "Bölüm Sayısı" : "Oda Sayısı",
                ShowRooms = !isLand,
                ShowFloor = category == "housing",
                ShowZoning = isLand,
                ShowHousingCategory = category == "housing",
                ShowLandCategory = isLand,
                ShowBuildingAge = category == "housing" || (isCommercial && !isForSale),
                ShowHeating = category == "housing" || (isCommercial && !isForSale),
                ShowTitleDeed = isLand || (isCommercial && isForSale),
                ShowFurnished = category == "housing" && !isForSale,
                ShowWithinComplex = category == "housing",
                ShowLoanEligible = isForSale && (category == "housing" || isLand),
                ShowTransferable = isCommercial && isForSale,
                ShowPrice = true,
                ShowArea = true,
                ShowDistance = false
            };

            if (effectiveUsageType != expectedUsageType)
            {
                return result;
            }

            var facets = await _propertyRepository.GetSearchFilterFacetsAsync(
                effectiveUsageType,
                request.ListingTypeId,
                request.CityId,
                request.DistrictId);

            result.TotalCount = facets.TotalCount;
            result.MinPrice = facets.MinPrice ?? 0m;
            result.MaxPrice = facets.MaxPrice ?? 0m;
            result.MinArea = facets.MinArea ?? 0d;
            result.MaxArea = facets.MaxArea ?? 0d;

            result.AgentOptions = facets.Agents
                .Where(a => !string.IsNullOrWhiteSpace(a.Id) && !string.IsNullOrWhiteSpace(a.Text))
                .OrderBy(a => a.Text, TrComparer)
                .ToList();

            if (isLand)
            {
                result.ZoningOptions = facets.ZoningStatuses
                    .Where(z => !string.IsNullOrWhiteSpace(z))
                    .Select(z => z.Trim())
                    .Distinct(TrComparer)
                    .OrderBy(z => z, TrComparer)
                    .Select(z => new FilterOptionDto(z, z))
                    .ToList();
            }
            else if (isCommercial)
            {
                result.RoomOptions = facets.Sections
                    .Where(s => s > 0)
                    .Distinct()
                    .OrderBy(s => s)
                    .Select(s => new FilterOptionDto(s.ToString(CultureInfo.InvariantCulture), $"{s} Bölüm"))
                    .ToList();
            }
            else
            {
                result.RoomOptions = SortAndFilterRooms(facets.Rooms)
                    .Select(r => new FilterOptionDto(r, r))
                    .ToList();

                result.FloorOptions = facets.Floors
                    .Distinct()
                    .OrderBy(f => f)
                    .Select(f => new FilterOptionDto(f.ToString(CultureInfo.InvariantCulture), FormatFloor(f)))
                    .ToList();
            }

            if (result.ShowHousingCategory)
                result.HousingCategoryOptions = facets.HousingCategories.OrderBy(o => o.Text, TrComparer).ToList();
            if (result.ShowLandCategory)
                result.LandCategoryOptions = facets.LandCategories.OrderBy(o => o.Text, TrComparer).ToList();
            if (result.ShowBuildingAge)
                result.BuildingAgeOptions = ToOptions(facets.BuildingAges);
            if (result.ShowHeating)
                result.HeatingOptions = ToOptions(facets.Heatings);
            if (result.ShowTitleDeed)
                result.TitleDeedOptions = ToOptions(facets.TitleDeedStatuses);

            return result;
        }

        private static string FormatFloor(int floor)
        {
            if (floor == 0)
                return "Zemin Kat";
            if (floor < 0)
                return $"Bodrum Kat ({floor})";
            return $"{floor}. Kat";
        }

        private static IEnumerable<string> SortAndFilterRooms(IEnumerable<string> rooms)
        {
            var parsed = new List<(string Raw, bool HasMain, int Main, int Sub)>();

            foreach (var raw in rooms.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).Distinct(TrComparer))
            {
                var parts = raw.Split('+');
                var hasMain = int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var main);
                var sub = 0;
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out sub);
                }

                if (hasMain && main <= 0)
                    continue;

                parsed.Add((raw, hasMain, main, sub));
            }

            return parsed
                .OrderBy(x => x.HasMain ? 0 : 1)
                .ThenBy(x => x.Main)
                .ThenBy(x => x.Sub)
                .ThenBy(x => x.Raw, TrComparer)
                .Select(x => x.Raw);
        }

        private static List<FilterOptionDto> ToOptions(IEnumerable<string> values)
        {
            return values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(TrComparer)
                .OrderBy(x => x, TrComparer)
                .Select(x => new FilterOptionDto(x, x))
                .ToList();
        }
    }
}

