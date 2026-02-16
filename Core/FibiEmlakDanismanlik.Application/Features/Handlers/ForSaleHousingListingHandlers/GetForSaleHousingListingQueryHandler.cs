using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleHousingListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSaleHousingListingResult;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleHousingListingHandlers
{
    public class GetForSaleHousingListingQueryHandler : IRequestHandler<GetForSaleHousingPropertyListingQuery, List<GetForSaleHousingListingResult>>
    {
        private readonly IRepository<ForSaleHousingListing> _repository;

        public GetForSaleHousingListingQueryHandler(IRepository<ForSaleHousingListing> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetForSaleHousingListingResult>> Handle(GetForSaleHousingPropertyListingQuery request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetAllAsync();
            return item.Select(x=> new GetForSaleHousingListingResult{

                PropertyNo = x.PropertyNo,
                ForSaleHousingListId = x.ForSaleHousingListId,
                PropertyName = x.PropertyName,
                PropertyStatus = x.PropertyStatus,
                PropertyDescription = x.PropertyDescription,
                Price = x.Price,
                CreatedDate = x.CreatedDate,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                AddressDesc = x.AddressDesc,
                Facade = x.Facade,
                IsElevator = x.IsElevator,
                GrossArea = x.GrossArea,
                NetArea = x.NetArea,
                OpenArea = x.OpenArea,
                BuildingAge = x.BuildingAge,
                TotalNumberOfFloor = x.TotalNumberOfFloor,
                NumberOfFloors = x.NumberOfFloors,
                NumberOfBathRoom = x.NumberOfBathRoom,
                NumberOfRoom = x.NumberOfRoom,
                NumberOfBalconies = x.NumberOfBalconies,
                Heating=x.Heating,
                BlackBox=x.BlackBox,
                ParkingLot=x.ParkingLot,
                Furnished=x.Furnished,
                UsageStatus=x.UsageStatus,
                WithinTheComplex=x.WithinTheComplex,
                Dues=x.Dues,
                Exchange=x.Exchange,
                TitleDeedStatus=x.TitleDeedStatus,
                HomeLoan=x.HomeLoan,
                BestDeals=x.BestDeals,
                HousingCategoryId=x.HousingCategoryId,
                AgentId=x.AgentId,
                
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
