using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleLandListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleLandListingResults;
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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleLandListingHandlers
{
    public class GetForSaleLandListingQueryHandler : IRequestHandler<GetForSaleLandListingQuery, List<GetForSalesLandListingResult>>
    {
        private readonly IRepository<ForSaleLandListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForSaleLandListingQueryHandler(IRepository<ForSaleLandListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetForSalesLandListingResult>> Handle(GetForSaleLandListingQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            var results = values.Select(x => new GetForSalesLandListingResult
            {
                AddressDesc = x.AddressDesc,
                AgentId = x.AgentId,
                Area = x.Area,
                BaseAreaRatio = x.BaseAreaRatio,
                BestDeals = x.BestDeals,
                City = x.City,
                CreatedDate = x.CreatedDate,
                DevelopmentRight = x.DevelopmentRight,
                District = x.District,
                Exchange = x.Exchange,
                FloorAreaRatio = x.FloorAreaRatio,
                ForSaleLandListingId = x.ForSaleLandListingId,
                LandCategoryId = x.LandCategoryId,
                LandLoan = x.LandLoan,
                MapSheetNumber = x.MapSheetNumber,
                Neighborhood = x.Neighborhood,
                ParcelNumber = x.ParcelNumber,
                PlotNumber = x.PlotNumber,
                Price = x.Price,
                PricePerSquareMeter = x.PricePerSquareMeter,
                PropertyDescription = x.PropertyDescription,
                PropertyName = x.PropertyName,
                PropertyNo = x.PropertyNo,
                PropertyStatus = x.PropertyStatus,
                SharePercentage = x.SharePercentage,
                TitleDeedStatus = x.TitleDeedStatus,
                ZoningPlan = x.ZoningPlan,
                ZoningStatus = x.ZoningStatus
            }).ToList();

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(
                results.Select(r => (1, r.ForSaleLandListingId)));

            foreach (var result in results)
            {
                ListingImageSlots.Apply(result, map.TryGetValue((1, result.ForSaleLandListingId), out var urls) ? urls : null);
            }

            return results;
        }
    }
}
