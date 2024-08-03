using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public CreateForSaleCommercialPropertyListingCommandHandler(IRepository<ForSaleCommercialPropertyListing> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateForSaleCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new ForSaleCommercialPropertyListing
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
                PropertyStatus=request.PropertyStatus,
                CreatedDate = DateTime.UtcNow,
                //images
                PropImgUrl1 = request.PropImgUrl1,
                PropImgUrl2 = request.PropImgUrl2,
                PropImgUrl3 = request.PropImgUrl3,
                PropImgUrl4 = request.PropImgUrl4,
                PropImgUrl5 = request.PropImgUrl5,
                PropImgUrl6 = request.PropImgUrl6,
                PropImgUrl7 = request.PropImgUrl7,
                PropImgUrl8 = request.PropImgUrl8,
                PropImgUrl9 = request.PropImgUrl9,
                PropImgUrl10 = request.PropImgUrl10,
                PropImgUrl11 = request.PropImgUrl11,
                PropImgUrl12 = request.PropImgUrl12,
                PropImgUrl13 = request.PropImgUrl13,
                PropImgUrl14 = request.PropImgUrl14,
                PropImgUrl15 = request.PropImgUrl15,
                PropImgUrl16 = request.PropImgUrl16,
                PropImgUrl17 = request.PropImgUrl17,
                PropImgUrl18 = request.PropImgUrl18,
                PropImgUrl19 = request.PropImgUrl19,
                PropImgUrl20 = request.PropImgUrl20,
                PropImgUrl21 = request.PropImgUrl21,
                PropImgUrl22 = request.PropImgUrl22,
                PropImgUrl23 = request.PropImgUrl23,
                PropImgUrl24 = request.PropImgUrl24,
                PropImgUrl25 = request.PropImgUrl25,
                PropImgUrl26 = request.PropImgUrl26,
                PropImgUrl27 = request.PropImgUrl27,
                PropImgUrl28 = request.PropImgUrl28,
                PropImgUrl29 = request.PropImgUrl29,
                PropImgUrl30 = request.PropImgUrl30
                //images


            });


        }
    }
}
