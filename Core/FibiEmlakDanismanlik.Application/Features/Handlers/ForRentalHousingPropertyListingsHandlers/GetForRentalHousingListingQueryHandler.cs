using AutoMapper;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalHousingListingQueries;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalHousingListingResults;
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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalHousingPropertyListingsHandlers
{
    public class GetForRentalHousingListingQueryHandler : IRequestHandler<GetRentalHousingPropertyListingQueries, List<GetRentalHousingPropertyListingResult>>
    {
        private readonly IRepository<RentalHousingListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForRentalHousingListingQueryHandler(IRepository<RentalHousingListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetRentalHousingPropertyListingResult>> Handle(GetRentalHousingPropertyListingQueries request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            var results = value.Select(x => new GetRentalHousingPropertyListingResult
            {
                PropertyNo = x.PropertyNo,
                RentalHousingListId = x.RentalHousingListId,
                PropertyName = x.PropertyName,
                PropertyStatus = x.PropertyStatus,
                PropertyDescription = x.PropertyDescription,
                Rent = x.Rent,
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
                WithinTheComplex = x.WithinTheComplex,
                Dues = x.Dues,
                BestDeals = x.BestDeals,
                HousingCategoryId = x.HousingCategoryId,
                AgentId = x.AgentId,
                ListingTypeId = x.ListingTypeId,
                Deposit = x.Deposit,
                HousingCategory = x.HousingCategory,
                ListingType = x.ListingType,
                Agent = x.Agent
            }).ToList();

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(
                results.Select(r => (8, r.RentalHousingListId)));

            foreach (var result in results)
            {
                ListingImageSlots.Apply(result, map.TryGetValue((8, result.RentalHousingListId), out var urls) ? urls : null);
            }

            return results;
        }
    }
}
