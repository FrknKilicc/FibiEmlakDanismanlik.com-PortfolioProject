using AutoMapper;
using FibiEmlakDanismanlik.Application.Features.Commands.RentalHousingPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalHousingListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalHousingListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public CreateHousingCommandHandler(IMapper mapper, IRepository<RentalHousingListing> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task Handle(CreateRentalHousingPropertyListingCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new RentalHousingListing
            {
                AddressDesc = request.AddressDesc,
                AgentId = request.AgentId,
                BestDeals = request.BestDeals,
                BlackBox = request.BlackBox,
                BuildingAge = request.BuildingAge,
                City = request.City,
                CreatedDate = request.CreatedDate,
                Deposit= request.Deposit,
                District= request.District,
                Dues= request.Dues,
                Facade= request.Facade,
                Furnished = request.Furnished,
                GrossArea = request.GrossArea, 
                Heating   = request.Heating,
                HousingCategoryId = request.HousingCategoryId,
                IsElevator = request.IsElevator,
                ListingTypeId = request.ListingTypeId,
                Neighborhood = request.Neighborhood,
                NetArea  = request.NetArea,
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
               
                //images
                PropImgUrl1 = request.PropImgUrl1,
                PropImgUrl2 = request.PropImgUrl2,
                PropImgUrl3 = request.PropImgUrl3,
                PropImgUrl4 = request.PropImgUrl4,
                PropImgUrl5 = request.PropImgUrl5,
                PropImgUrl6 = request.PropImgUrl6,
                PropImgUrl7 = request.PropImgUrl7,
                PropImgUrl8 = request.PropImgUrl8,
                PropImgUrl9 = request.PropImgUrl9,
                PropImgUrl10 = request.PropImgUrl10,
                PropImgUrl11 = request.PropImgUrl11,
                PropImgUrl12 = request.PropImgUrl12,
                PropImgUrl13 = request.PropImgUrl13,
                PropImgUrl14 = request.PropImgUrl14,
                PropImgUrl15 = request.PropImgUrl15,
                PropImgUrl16 = request.PropImgUrl16,
                PropImgUrl17 = request.PropImgUrl17,
                PropImgUrl18 = request.PropImgUrl18,
                PropImgUrl19 = request.PropImgUrl19,
                PropImgUrl20 = request.PropImgUrl20,
                PropImgUrl21 = request.PropImgUrl21,
                PropImgUrl22 = request.PropImgUrl22,
                PropImgUrl23 = request.PropImgUrl23,
                PropImgUrl24 = request.PropImgUrl24,
                PropImgUrl25 = request.PropImgUrl25,
                PropImgUrl26 = request.PropImgUrl26,
                PropImgUrl27 = request.PropImgUrl27,
                PropImgUrl28 = request.PropImgUrl28,
                PropImgUrl29 = request.PropImgUrl29,
                PropImgUrl30 = request.PropImgUrl30
                //images




            });
        }
    }
}
