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
    public class GetForSaleCommercialPropertyListingQueryHandler : IRequestHandler<GetForSaleCommercialPropertyListingQuery, List<GetForSaleCommercialPropertyListingResult>>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForSaleCommercialPropertyListingQueryHandler(IRepository<ForSaleCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetForSaleCommercialPropertyListingResult>> Handle(GetForSaleCommercialPropertyListingQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            var results = value.Select(x => new GetForSaleCommercialPropertyListingResult
            {
                ForSaleCommercialListingId = x.ForSaleCommercialListingId,
                PropertyNo = x.PropertyNo,
                AddressDesc = x.AddressDesc,
                AgentId = x.AgentId,
                Area = x.Area,
                BestDeals = x.BestDeals,
                City = x.City,
                District = x.District,
                Exchange = x.Exchange,
                Facade = x.Facade,
                GrossArea = x.GrossArea,
                LandLoan = x.LandLoan,
                Neighborhood = x.Neighborhood,
                NumberOfBathrooms = x.NumberOfBathrooms,
                NumberOfFloors = x.NumberOfFloors,
                NumberOfKitchens = x.NumberOfKitchens,
                NumberOfSection = x.NumberOfSection,
                Price = x.Price,
                PropertyDescription = x.PropertyDescription,
                PropertyName = x.PropertyName,
                SharePercentage = x.SharePercentage,
                TitleDeedStatus = x.TitleDeedStatus,
                Transferable = x.Transferable,
                CreatedDate = x.CreatedDate,
                PropertyStatus = x.PropertyStatus
            }).ToList();

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(
                results.Select(r => (3, r.ForSaleCommercialListingId)));

            foreach (var result in results)
            {
                ListingImageSlots.Apply(result, map.TryGetValue((3, result.ForSaleCommercialListingId), out var urls) ? urls : null);
            }

            return results;
        }
    }
}
