using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleHousingCommands;
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
    public class CreateForSaleHousingCommandHandler : IRequestHandler<CreateForSaleHousingCommand>
    {
        private readonly IRepository<ForSaleHousingListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public CreateForSaleHousingCommandHandler(IRepository<ForSaleHousingListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task Handle(CreateForSaleHousingCommand request, CancellationToken cancellationToken)
        {
            var entity = new ForSaleHousingListing
            {
                PropertyName = request.PropertyName,
                PropertyDescription = request.PropertyDescription,
                PropertyStatus = request.PropertyStatus,
                CreatedDate = DateTime.UtcNow,
                Price = request.Price,
                City = request.City,
                District = request.District,
                Neighborhood = request.Neighborhood,
                AddressDesc = request.AddressDesc,
                Facade = request.Facade,
                IsElevator = request.IsElevator,
                GrossArea = request.GrossArea,
                NetArea = request.NetArea,
                OpenArea = request.OpenArea,
                BuildingAge = request.BuildingAge,
                TotalNumberOfFloor = request.TotalNumberOfFloor,
                NumberOfBathRoom = request.NumberOfBathRoom,
                NumberOfRoom = request.NumberOfRoom,
                Heating = request.Heating,
                BlackBox = request.BlackBox,
                NumberOfBalconies = request.NumberOfBalconies,
                ParkingLot = request.ParkingLot,
                Furnished = request.Furnished,
                UsageStatus = request.UsageStatus,
                WithinTheComplex = request.WithinTheComplex,
                Dues = request.Dues,
                Exchange = request.Exchange,
                TitleDeedStatus = request.TitleDeedStatus,
                HomeLoan = request.HomeLoan,
                BestDeals = request.BestDeals,
                HousingCategoryId = request.HousingCategoryId,
                AgentId = request.AgentId,
            };

            await _repository.CreateAsync(entity);

            await _listingImageRepository.ReplacePhotoUrlsAsync(2, entity.ForSaleHousingListId, new[]
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
