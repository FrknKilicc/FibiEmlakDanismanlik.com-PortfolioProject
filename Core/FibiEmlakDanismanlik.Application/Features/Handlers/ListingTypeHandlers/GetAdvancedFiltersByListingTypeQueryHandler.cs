using FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingTypeHandlers
{
    public class GetAdvancedFiltersByListingTypeQueryHandler : IRequestHandler<GetAdvancedFiltersByListingTypeQuery, AdvancedFiltersVisibilityDto>
    {
        private readonly IListingTypeRepository _listingTypeRepository;

        public GetAdvancedFiltersByListingTypeQueryHandler(IListingTypeRepository listingTypeRepository)
        {
            _listingTypeRepository = listingTypeRepository;
        }

        public async Task<AdvancedFiltersVisibilityDto> Handle(GetAdvancedFiltersByListingTypeQuery request, CancellationToken cancellationToken)
        {
            var listingType = await _listingTypeRepository.GetByIdAsync(request.ListingTypeId);
            
            var typeName = listingType?.Name ?? string.Empty;
            var isLand = typeName.Contains("Arsa", System.StringComparison.OrdinalIgnoreCase);
            var isCommercial = typeName.Contains("İş Yeri", System.StringComparison.OrdinalIgnoreCase)
                            || typeName.Contains("Is Yeri", System.StringComparison.OrdinalIgnoreCase)
                            || typeName.Contains("Ticari", System.StringComparison.OrdinalIgnoreCase);
            var isRental = listingType != null && listingType.UsageType == 2;

            var result = new AdvancedFiltersVisibilityDto
            {
                ShowRooms = !isLand,
                ShowBath = !isLand,
                ShowFloor = !isLand,
                ShowPrice = true,
                ShowArea = true,
                ShowDistance = true
            };

            if (isLand)
            {
                result.RoomOptions = new List<FilterOptionDto>();
                result.BathOptions = new List<FilterOptionDto>();
                result.FloorOptions = new List<FilterOptionDto>();
                result.MinPrice = 0;
                result.MaxPrice = isRental ? 500_000 : 100_000_000;
                result.MinArea = 0;
                result.MaxArea = 50_000;
            }
            else if (isCommercial)
            {
                result.RoomOptions = new List<FilterOptionDto>
                {
                    new("1", "1 Bölüm"),
                    new("2", "2 Bölüm"),
                    new("3", "3 Bölüm"),
                    new("4", "4 ve Üzeri Bölüm")
                };
                result.BathOptions = new List<FilterOptionDto>
                {
                    new("1", "1 Lavabo"),
                    new("2", "2 Lavabo"),
                    new("3", "3 ve Üzeri Lavabo")
                };
                result.FloorOptions = new List<FilterOptionDto>
                {
                    new("0", "Zemin Kat"),
                    new("1", "1. Kat"),
                    new("2", "2. Kat"),
                    new("3", "3. Kat ve Üzeri")
                };
                result.MinPrice = 0;
                result.MaxPrice = isRental ? 500_000 : 75_000_000;
                result.MinArea = 0;
                result.MaxArea = 10_000;
            }
            else // Konut / Housing / Default
            {
                result.RoomOptions = new List<FilterOptionDto>
                {
                    new("1", "1 Oda"),
                    new("2", "2 Oda"),
                    new("3", "3 Oda"),
                    new("4", "4 ve Üzeri Oda")
                };
                result.BathOptions = new List<FilterOptionDto>
                {
                    new("1", "1 Banyo"),
                    new("2", "2 Banyo"),
                    new("3", "3 Banyo"),
                    new("4", "4 ve Üzeri Banyo")
                };
                result.FloorOptions = new List<FilterOptionDto>
                {
                    new("0", "Zemin Kat"),
                    new("1", "1. Kat"),
                    new("2", "2. Kat"),
                    new("3", "3. Kat"),
                    new("4", "4. Kat ve Üzeri")
                };
                result.MinPrice = 0;
                result.MaxPrice = isRental ? 250_000 : 50_000_000;
                result.MinArea = 0;
                result.MaxArea = 5_000;
            }

            return result;
        }
    }
}

