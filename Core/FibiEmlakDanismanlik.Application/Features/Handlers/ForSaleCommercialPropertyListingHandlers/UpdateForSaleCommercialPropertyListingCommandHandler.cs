using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleCommercialPropertyListingHandlers
{
    public class UpdateForSaleCommercialPropertyListingCommandHandler : IRequestHandler<UpdateForSaleCommercialPropertyListingCommand>
    {
        private readonly IRepository<ForSaleCommercialPropertyListing> _repository;

        public UpdateForSaleCommercialPropertyListingCommandHandler(IRepository<ForSaleCommercialPropertyListing> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateForSaleCommercialPropertyListingCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.ForSaleCommercialListingId);
            value.AddressDesc = request.AddressDesc;
            value.AgentId = request.AgentId;
            value.Area = request.Area;
            value.BestDeals = request.BestDeals;
            value.City = request.City;
            value.District = request.District;
            value.Exchange = request.Exchange;
            value.Facade = request.Facade;
            value.GrossArea = request.GrossArea;
            value.LandLoan = request.LandLoan;
            value.Neighborhood = request.Neighborhood;
            value.NumberOfBathrooms = request.NumberOfBathrooms;
            value.NumberOfFloors = request.NumberOfFloors;
            value.NumberOfKitchens = request.NumberOfKitchens;
            value.NumberOfSection = request.NumberOfSection;
            value.Price = request.Price;
            value.PropertyDescription = request.PropertyDescription;
            value.PropertyName = request.PropertyName;
            value.SharePercentage = request.SharePercentage;
            value.TitleDeedStatus = request.TitleDeedStatus;
            value.Transferable = request.Transferable;
            // images
            value.PropImgUrl1 = request.PropImgUrl1;
            value.PropImgUrl2 = request.PropImgUrl2;
            value.PropImgUrl3 = request.PropImgUrl3;
            value.PropImgUrl4 = request.PropImgUrl4;
            value.PropImgUrl5 = request.PropImgUrl5;
            value.PropImgUrl6 = request.PropImgUrl6;
            value.PropImgUrl7 = request.PropImgUrl7;
            value.PropImgUrl8 = request.PropImgUrl8;
            value.PropImgUrl9 = request.PropImgUrl9;
            value.PropImgUrl10 = request.PropImgUrl10;
            value.PropImgUrl11 = request.PropImgUrl11;
            value.PropImgUrl12 = request.PropImgUrl12;
            value.PropImgUrl13 = request.PropImgUrl13;
            value.PropImgUrl14 = request.PropImgUrl14;
            value.PropImgUrl15 = request.PropImgUrl15;
            value.PropImgUrl16 = request.PropImgUrl16;
            value.PropImgUrl17 = request.PropImgUrl17;
            value.PropImgUrl18 = request.PropImgUrl18;
            value.PropImgUrl19 = request.PropImgUrl19;
            value.PropImgUrl20 = request.PropImgUrl20;
            value.PropImgUrl21 = request.PropImgUrl21;
            value.PropImgUrl22 = request.PropImgUrl22;
            value.PropImgUrl23 = request.PropImgUrl23;
            value.PropImgUrl24 = request.PropImgUrl24;
            value.PropImgUrl25 = request.PropImgUrl25;
            value.PropImgUrl26 = request.PropImgUrl26;
            value.PropImgUrl27 = request.PropImgUrl27;
            value.PropImgUrl28 = request.PropImgUrl28;
            value.PropImgUrl29 = request.PropImgUrl29;
            value.PropImgUrl30 = request.PropImgUrl30;
            // images

            await _repository.UpdateAsync(value);


        }
    }
}
