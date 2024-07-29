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

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleLandListingHandlers
{
    public class GetForSaleLandListingQueryHandler : IRequestHandler<GetForSaleLandListingQuery, List<GetForSalesLandListingResult>>
    {
        private readonly IRepository<ForSaleLandListing> _repository;

        public GetForSaleLandListingQueryHandler(IRepository<ForSaleLandListing> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetForSalesLandListingResult>> Handle(GetForSaleLandListingQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            return values.Select(x=> new GetForSalesLandListingResult
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
              Exchange= x.Exchange,
              FloorAreaRatio = x.FloorAreaRatio,
              ForSaleLandListingId = x.ForSaleLandListingId,
              LandCategoryId = x.LandCategoryId,
              LandLoan=x.LandLoan,
              MapSheetNumber=x.MapSheetNumber,
              Neighborhood=x.Neighborhood, 
              ParcelNumber=x.ParcelNumber, 
              PlotNumber=x.PlotNumber,
              Price=x.Price,
              PricePerSquareMeter=x.PricePerSquareMeter,
              PropertyDescription=x.PropertyDescription,
              PropertyName=x.PropertyName,
              PropertyNo=x.PropertyNo,
              PropertyStatus=x.PropertyStatus,
              SharePercentage=x.SharePercentage,
              TitleDeedStatus=x.TitleDeedStatus,
              ZoningPlan=x.ZoningPlan,
              ZoningStatus=x.ZoningStatus,
                //images
                PropImgUrl1 = x.PropImgUrl1,
                PropImgUrl2 = x.PropImgUrl2,
                PropImgUrl3 = x.PropImgUrl3,
                PropImgUrl4 = x.PropImgUrl4,
                PropImgUrl5 = x.PropImgUrl5,
                PropImgUrl6 = x.PropImgUrl6,
                PropImgUrl7 = x.PropImgUrl7,
                PropImgUrl8 = x.PropImgUrl8,
                PropImgUrl9 = x.PropImgUrl9,
                PropImgUrl10 = x.PropImgUrl10,
                PropImgUrl11 = x.PropImgUrl11,
                PropImgUrl12 = x.PropImgUrl12,
                PropImgUrl13 = x.PropImgUrl13,
                PropImgUrl14 = x.PropImgUrl14,
                PropImgUrl15 = x.PropImgUrl15,
                PropImgUrl16 = x.PropImgUrl16,
                PropImgUrl17 = x.PropImgUrl17,
                PropImgUrl18 = x.PropImgUrl18,
                PropImgUrl19 = x.PropImgUrl19,
                PropImgUrl20 = x.PropImgUrl20,
                PropImgUrl21 = x.PropImgUrl21,
                PropImgUrl22 = x.PropImgUrl22,
                PropImgUrl23 = x.PropImgUrl23,
                PropImgUrl24 = x.PropImgUrl24,
                PropImgUrl25 = x.PropImgUrl25,
                PropImgUrl26 = x.PropImgUrl26,
                PropImgUrl27 = x.PropImgUrl27,
                PropImgUrl28 = x.PropImgUrl28,
                PropImgUrl29 = x.PropImgUrl29,
                PropImgUrl30 = x.PropImgUrl30

            }).ToList();
        }
    }
}
