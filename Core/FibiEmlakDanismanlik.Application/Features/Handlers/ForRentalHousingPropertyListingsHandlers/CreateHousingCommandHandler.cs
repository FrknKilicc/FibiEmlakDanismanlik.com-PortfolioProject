using AutoMapper;
using FibiEmlakDanismanlik.Application.Features.Commands.RentalHousingPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalHousingListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalHousingListingResults;
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
    public class CreateHousingCommandHandler : IRequestHandler<CreateRentalHousingPropertyListingCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<RentalHousingListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public CreateHousingCommandHandler(IMapper mapper, IRepository<RentalHousingListing> repository, IListingImageRepository listingImageRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task Handle(CreateRentalHousingPropertyListingCommand request, CancellationToken cancellationToken)
        {
            var entity = new RentalHousingListing
            {
                AddressDesc = request.AddressDesc,
                AgentId = request.AgentId,
                BestDeals = request.BestDeals,
                BlackBox = request.BlackBox,
                BuildingAge = request.BuildingAge,
                City = request.City,
                CreatedDate = request.CreatedDate,
                Deposit = request.Deposit,
                District = request.District,
                Dues = request.Dues,
                Facade = request.Facade,
                Furnished = request.Furnished,
                GrossArea = request.GrossArea,
                Heating = request.Heating,
                HousingCategoryId = request.HousingCategoryId,
                IsElevator = request.IsElevator,
                ListingTypeId = request.ListingTypeId,
                Neighborhood = request.Neighborhood,
                NetArea = request.NetArea,
                NumberOfBalconies = request.NumberOfBalconies,
                NumberOfBathRoom = request.NumberOfBathRoom,
                NumberOfFloors = request.NumberOfFloors,
                NumberOfRoom = request.NumberOfRoom,
                OpenArea = request.OpenArea,
                ParkingLot = request.ParkingLot,
                PropertyName = request.PropertyName,
                PropertyDescription = request.PropertyDescription,
                PropertyStatus = request.PropertyStatus,
                Rent = request.Rent,
                TotalNumberOfFloor = request.TotalNumberOfFloor,
                WithinTheComplex = request.WithinTheComplex,
            };

            await _repository.CreateAsync(entity);

            await _listingImageRepository.ReplacePhotoUrlsAsync(8, entity.RentalHousingListId, new[]
            {
                request.PropImgUrl1,
                request.PropImgUrl2,
                request.PropImgUrl3,
                request.PropImgUrl4,
                request.PropImgUrl5,
                request.PropImgUrl6,
                request.PropImgUrl7,
                request.PropImgUrl8,
                request.PropImgUrl9,
                request.PropImgUrl10,
                request.PropImgUrl11,
                request.PropImgUrl12,
                request.PropImgUrl13,
                request.PropImgUrl14,
                request.PropImgUrl15,
                request.PropImgUrl16,
                request.PropImgUrl17,
                request.PropImgUrl18,
                request.PropImgUrl19,
                request.PropImgUrl20,
                request.PropImgUrl21,
                request.PropImgUrl22,
                request.PropImgUrl23,
                request.PropImgUrl24,
                request.PropImgUrl25,
                request.PropImgUrl26,
                request.PropImgUrl27,
                request.PropImgUrl28,
                request.PropImgUrl29,
                request.PropImgUrl30
            });
        }
    }
}
