using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleCommercialPropertyListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public GetForSaleCommercialPropertyListingByIdQueryHandler(IRepository<ForSaleCommercialPropertyListing> repository)
        {
            _repository = repository;
        }

        public async Task<GetForSaleCommercialPropertyListingByIdResult> Handle(GetForSaleCommercialPropertListingByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetForSaleCommercialPropertyListingByIdResult
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
                //images
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
                //images

            };
        }
    }
}
