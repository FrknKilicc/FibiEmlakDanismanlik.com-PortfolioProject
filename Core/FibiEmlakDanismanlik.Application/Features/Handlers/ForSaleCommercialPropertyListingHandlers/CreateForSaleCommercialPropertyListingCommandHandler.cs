using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class CreateForSaleCommercialPropertyListingCommandHandler : IRequestHandler<CreateForSaleCommercialPropertyListingCommand>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public CreateForSaleCommercialPropertyListingCommandHandler(IRepository<ForSaleCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task Handle(CreateForSaleCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
            var entity = new ForSaleCommercialPropertyListing
            {
                AddressDesc = request.AddressDesc,
                AgentId = request.AgentId,
                Area = request.Area,
                BestDeals = request.BestDeals,
                City = request.City,
                District = request.District,
                Exchange = request.Exchange,
                Facade = request.Facade,
                GrossArea = request.GrossArea,
                LandLoan = request.LandLoan,
                Neighborhood = request.Neighborhood,
                NumberOfBathrooms = request.NumberOfBathrooms,
                NumberOfFloors = request.NumberOfFloors,
                NumberOfKitchens = request.NumberOfKitchens,
                NumberOfSection = request.NumberOfSection,
                Price = request.Price,
                PropertyDescription = request.PropertyDescription,
                PropertyName = request.PropertyName,
                SharePercentage = request.SharePercentage,
                TitleDeedStatus = request.TitleDeedStatus,
                Transferable = request.Transferable,
                PropertyStatus = PropertyStatusHelper.NormalizeAndValidate(request.PropertyStatus),
                CreatedDate = DateTime.UtcNow,
            };

            await _repository.CreateAsync(entity);

            await _listingImageRepository.ReplacePhotoUrlsAsync(3, entity.ForSaleCommercialListingId, new[]
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
