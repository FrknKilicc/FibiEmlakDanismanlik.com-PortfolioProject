using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class UpdateForSaleCommercialPropertyListingCommandHandler : IRequestHandler<UpdateForSaleCommercialPropertyListingCommand>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public UpdateForSaleCommercialPropertyListingCommandHandler(IRepository<ForSaleCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task Handle(UpdateForSaleCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.ForSaleCommercialListingId);
            if (value == null)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }
            value.AddressDesc = request.AddressDesc;
            value.AgentId = request.AgentId;
            value.Area = request.Area;
            value.BestDeals = request.BestDeals;
            value.City = request.City;
            value.District = request.District;
            value.Exchange = request.Exchange;
            value.Facade = request.Facade;
            value.GrossArea = request.GrossArea;
            value.LandLoan = request.LandLoan;
            value.Neighborhood = request.Neighborhood;
            value.NumberOfBathrooms = request.NumberOfBathrooms;
            value.NumberOfFloors = request.NumberOfFloors;
            value.NumberOfKitchens = request.NumberOfKitchens;
            value.NumberOfSection = request.NumberOfSection;
            value.Price = request.Price;
            value.PropertyDescription = request.PropertyDescription;
            value.PropertyName = request.PropertyName;
            value.SharePercentage = request.SharePercentage;
            value.TitleDeedStatus = request.TitleDeedStatus;
            value.Transferable = request.Transferable;
            value.PropertyStatus = request.PropertyStatus;

            await _repository.UpdateAsync(value);

            await _listingImageRepository.ReplacePhotoUrlsAsync(3, value.ForSaleCommercialListingId, new[]
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
