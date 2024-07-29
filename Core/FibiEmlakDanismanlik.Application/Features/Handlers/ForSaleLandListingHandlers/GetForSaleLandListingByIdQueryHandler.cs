using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleLandListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleLandListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public GetForSaleLandListingByIdQueryHandler(IRepository<ForSaleLandListing> repository)
        {
            _repository = repository;
        }

        public async Task<GetForSalesLandListingByIdResult> Handle(GetForSaleLandListingByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetForSalesLandListingByIdResult
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
                TitleDeedStatus = value.TitleDeedStatus,
                PropImgUrl1 = value.PropImgUrl1,
                PropImgUrl2 = value.PropImgUrl2,
                PropImgUrl3 = value.PropImgUrl3,
                PropImgUrl4 = value.PropImgUrl4,
                PropImgUrl5 = value.PropImgUrl5,
                PropImgUrl6 = value.PropImgUrl6,
                PropImgUrl7 = value.PropImgUrl7,
                PropImgUrl8 = value.PropImgUrl8,
                PropImgUrl9 = value.PropImgUrl9,
                PropImgUrl10 = value.PropImgUrl10,
                PropImgUrl11 = value.PropImgUrl11,
                PropImgUrl12 = value.PropImgUrl12,
                PropImgUrl13 = value.PropImgUrl13,
                PropImgUrl14 = value.PropImgUrl14,
                PropImgUrl15 = value.PropImgUrl15,
                PropImgUrl16 = value.PropImgUrl16,
                PropImgUrl17 = value.PropImgUrl17,
                PropImgUrl18 = value.PropImgUrl18,
                PropImgUrl19 = value.PropImgUrl19,
                PropImgUrl20 = value.PropImgUrl20,
                PropImgUrl21 = value.PropImgUrl21,
                PropImgUrl22 = value.PropImgUrl22,
                PropImgUrl23 = value.PropImgUrl23,
                PropImgUrl24 = value.PropImgUrl24,
                PropImgUrl25 = value.PropImgUrl25,
                PropImgUrl26 = value.PropImgUrl26,
                PropImgUrl27 = value.PropImgUrl27,
                PropImgUrl28 = value.PropImgUrl28,
                PropImgUrl29 = value.PropImgUrl29,
                PropImgUrl30 = value.PropImgUrl30
            };
        }
    }
}
