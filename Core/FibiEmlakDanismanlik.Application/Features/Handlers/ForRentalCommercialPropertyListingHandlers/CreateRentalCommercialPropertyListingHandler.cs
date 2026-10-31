using FibiEmlakDanismanlik.Application.Features.Commands.RentalCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.RentalCommercialPropertyListingHandlers
{
    public class CreateRentalCommercialPropertyListingHandler : IRequestHandler<CreateRentalCommercialPropertyListingCommand>
    {
        private readonly IRepository<RentalCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public CreateRentalCommercialPropertyListingHandler(IRepository<RentalCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task Handle(CreateRentalCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
            var entity = new RentalCommercialPropertyListing
            {
                AddressDesc = request.AddressDesc,
                AgentId = request.AgentId,
                BestDeals = request.BestDeals,
                BuildingAge = request.BuildingAge,
                City = request.City,
                CreatedDate = TurkeyClock.Today,
                Deposit = request.Deposit,
                District = request.District,
                Facade = request.Facade,
                GrossArea = request.GrossArea,
                Heating = request.Heating,
                Neighborhood = request.Neighborhood,
                NetArea = request.NetArea,
                NumberOfBathrooms = request.NumberOfBathrooms,
                NumberOfFloors = request.NumberOfFloors,
                NumberOfKitchens = request.NumberOfKitchens,
                NumberOfSection = request.NumberOfSection,
                PropertyDescription = request.PropertyDescription,
                PropertyName = request.PropertyName,
                PropertyStatus = request.PropertyName,
                Rent = request.Rent,
                TitleDeedStatus = request.TitleDeedStatus,
            };

            await _repository.CreateAsync(entity);

            await _listingImageRepository.ReplacePhotoUrlsAsync(9, entity.RentalCommercialListId, new[]
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
