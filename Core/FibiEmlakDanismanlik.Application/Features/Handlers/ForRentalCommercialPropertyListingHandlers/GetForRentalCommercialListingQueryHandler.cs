using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Queries.RentalCommercialPropertyListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.RentalCommercialPropertyListingResults;
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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalCommercialPropertyListingHandlers
{
    public class GetForRentalCommercialListingQueryHandler : IRequestHandler<GetRentalCommercialPropertyListingQuery, List<GetRentalCommercialPropertyListingResult>>
    {
        private readonly IRepository<RentalCommercialPropertyListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForRentalCommercialListingQueryHandler(IRepository<RentalCommercialPropertyListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetRentalCommercialPropertyListingResult>> Handle(GetRentalCommercialPropertyListingQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            var results = value.Select(x => new GetRentalCommercialPropertyListingResult
            {
                RentalCommercialListId = x.RentalCommercialListId,
                PropertyNo = x.PropertyNo,
                AddressDesc = x.AddressDesc,
                AgentId = x.AgentId,
                NetArea = x.NetArea,
                BestDeals = x.BestDeals,
                City = x.City,
                District = x.District,
                Facade = x.Facade,
                GrossArea = x.GrossArea,
                Neighborhood = x.Neighborhood,
                NumberOfBathrooms = x.NumberOfBathrooms,
                NumberOfFloors = x.NumberOfFloors,
                NumberOfKitchens = x.NumberOfKitchens,
                NumberOfSection = x.NumberOfSection,
                Rent = x.Rent,
                PropertyDescription = x.PropertyDescription,
                PropertyName = x.PropertyName,
                TitleDeedStatus = x.TitleDeedStatus,
                CreatedDate = x.CreatedDate,
                PropertyStatus = x.PropertyStatus,
                BuildingAge = x.BuildingAge,
                Deposit = x.Deposit,
                Heating = x.Heating
            }).ToList();

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(
                results.Select(r => (9, r.RentalCommercialListId)));

            foreach (var result in results)
            {
                ListingImageSlots.Apply(result, map.TryGetValue((9, result.RentalCommercialListId), out var urls) ? urls : null);
            }

            return results;
        }
    }
}
