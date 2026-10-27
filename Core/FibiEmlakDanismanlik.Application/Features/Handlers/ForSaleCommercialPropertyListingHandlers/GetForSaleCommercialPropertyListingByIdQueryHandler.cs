using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleCommercialPropertyListingResults;
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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class GetForSaleCommercialPropertyListingByIdQueryHandler : IRequestHandler<GetForSaleCommercialPropertListingByIdQuery, GetForSaleCommercialPropertyListingByIdResult>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForSaleCommercialPropertyListingByIdQueryHandler(IRepository<ForSaleCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<GetForSaleCommercialPropertyListingByIdResult> Handle(GetForSaleCommercialPropertListingByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }
            var result = new GetForSaleCommercialPropertyListingByIdResult
            {
                ForSaleCommercialListingId = value.ForSaleCommercialListingId,
                PropertyNo = value.PropertyNo,
                AddressDesc = value.AddressDesc,
                AgentId = value.AgentId,
                Area = value.Area,
                BestDeals = value.BestDeals,
                City = value.City,
                District = value.District,
                Exchange = value.Exchange,
                Facade = value.Facade,
                GrossArea = value.GrossArea,
                LandLoan = value.LandLoan,
                Neighborhood = value.Neighborhood,
                NumberOfBathrooms = value.NumberOfBathrooms,
                NumberOfFloors = value.NumberOfFloors,
                NumberOfKitchens = value.NumberOfKitchens,
                NumberOfSection = value.NumberOfSection,
                Price = value.Price,
                PropertyDescription = value.PropertyDescription,
                PropertyName = value.PropertyName,
                SharePercentage = value.SharePercentage,
                TitleDeedStatus = value.TitleDeedStatus,
                Transferable = value.Transferable,
                CreatedDate = value.CreatedDate,
                PropertyStatus = value.PropertyStatus
            };

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(new[] { (3, result.ForSaleCommercialListingId) });
            ListingImageSlots.Apply(result, map.TryGetValue((3, result.ForSaleCommercialListingId), out var urls) ? urls : null);

            return result;
        }
    }
}
