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
using System.Windows.Markup;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleLandListingHandlers
{
    public class GetForSaleLandListingByIdQueryHandler : IRequestHandler<GetForSaleLandListingByIdQuery, GetForSalesLandListingByIdResult>
    {
        private readonly IRepository<ForSaleLandListing> _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForSaleLandListingByIdQueryHandler(IRepository<ForSaleLandListing> repository, IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<GetForSalesLandListingByIdResult> Handle(GetForSaleLandListingByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }
            var result = new GetForSalesLandListingByIdResult
            {
                AddressDesc = value.AddressDesc,
                AgentId = value.AgentId,
                Area = value.Area,
                BaseAreaRatio = value.BaseAreaRatio,
                BestDeals = value.BestDeals,
                City = value.City,
                CreatedDate = value.CreatedDate,
                DevelopmentRight = value.DevelopmentRight,
                District = value.District,
                Exchange = value.Exchange,
                FloorAreaRatio = value.FloorAreaRatio,
                ForSaleLandListingId = value.ForSaleLandListingId,
                LandCategoryId = value.LandCategoryId,
                LandLoan = value.LandLoan,
                MapSheetNumber = value.MapSheetNumber,
                Neighborhood = value.Neighborhood,
                ParcelNumber = value.ParcelNumber,
                PlotNumber = value.PlotNumber,
                Price = value.Price,
                PricePerSquareMeter = value.PricePerSquareMeter,
                PropertyDescription = value.PropertyDescription,
                PropertyName = value.PropertyName,
                PropertyNo = value.PropertyNo,
                ZoningPlan = value.ZoningPlan,
                ZoningStatus = value.ZoningStatus,
                PropertyStatus = value.PropertyStatus,
                SharePercentage = value.SharePercentage,
                TitleDeedStatus = value.TitleDeedStatus
            };

            var map = await _listingImageRepository.GetPhotoUrlMapAsync(new[] { (1, result.ForSaleLandListingId) });
            ListingImageSlots.Apply(result, map.TryGetValue((1, result.ForSaleLandListingId), out var urls) ? urls : null);

            return result;
        }
    }
}
