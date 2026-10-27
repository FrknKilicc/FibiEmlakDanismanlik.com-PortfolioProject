using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleHousingListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleHousingListingResult;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleHousingListingHandlers
{
    public class GetForSaleHousingListingQueryHandler : IRequestHandler<GetForSaleHousingPropertyListingQuery, List<GetForSaleHousingListingResult>>
    {
        private readonly IRepository<ForSaleHousingListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForSaleHousingListingQueryHandler(IRepository<ForSaleHousingListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetForSaleHousingListingResult>> Handle(GetForSaleHousingPropertyListingQuery request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetAllAsync();
            var results = item.Select(x => new GetForSaleHousingListingResult
            {
                PropertyNo = x.PropertyNo,
                ForSaleHousingListId = x.ForSaleHousingListId,
                PropertyName = x.PropertyName,
                PropertyStatus = x.PropertyStatus,
                PropertyDescription = x.PropertyDescription,
                Price = x.Price,
                CreatedDate = x.CreatedDate,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                AddressDesc = x.AddressDesc,
                Facade = x.Facade,
                IsElevator = x.IsElevator,
                GrossArea = x.GrossArea,
                NetArea = x.NetArea,
                OpenArea = x.OpenArea,
                BuildingAge = x.BuildingAge,
                TotalNumberOfFloor = x.TotalNumberOfFloor,
                NumberOfFloors = x.NumberOfFloors,
                NumberOfBathRoom = x.NumberOfBathRoom,
                NumberOfRoom = x.NumberOfRoom,
                NumberOfBalconies = x.NumberOfBalconies,
                Heating = x.Heating,
                BlackBox = x.BlackBox,
                ParkingLot = x.ParkingLot,
                Furnished = x.Furnished,
                UsageStatus = x.UsageStatus,
                WithinTheComplex = x.WithinTheComplex,
                Dues = x.Dues,
                Exchange = x.Exchange,
                TitleDeedStatus = x.TitleDeedStatus,
                HomeLoan = x.HomeLoan,
                BestDeals = x.BestDeals,
                HousingCategoryId = x.HousingCategoryId,
                AgentId = x.AgentId
            }).ToList();

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(
                results.Select(r => (2, r.ForSaleHousingListId)));

            foreach (var result in results)
            {
                ListingImageSlots.Apply(result, map.TryGetValue((2, result.ForSaleHousingListId), out var urls) ? urls : null);
            }

            return results;
        }
    }
}
