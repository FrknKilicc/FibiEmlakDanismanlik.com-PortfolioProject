using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetAllForSalePropertyQueryHandler : IRequestHandler<GetAllForsalePropertyWithAgentQuery, List<GetAllForSalePropertyWithAgentResult>>
    {
        private readonly IPropertyRepository _propRepository;

        public GetAllForSalePropertyQueryHandler(IPropertyRepository propRepository)
        {
            _propRepository = propRepository;
        }

        public async Task<List<GetAllForSalePropertyWithAgentResult>> Handle(GetAllForsalePropertyWithAgentQuery request, CancellationToken cancellationToken)
        {
            var value = _propRepository.GetAllForSalePropertyWithAgent();
            return value.Select(x => new GetAllForSalePropertyWithAgentResult
            {
                AgentImgUrl = x.AgentImgUrl,
                AgentName = x.AgentName,
                AgentTitle = x.AgentTitle,
                Price = x.Price,
                PropertyDescription = x.PropertyDescription,
                PropertyName = x.PropertyName,
                PropertyStatus = x.PropertyStatus,
                CreatedDate = x.CreatedDate,
                PropertyType = x.PropertyType,
                PropImgUrl1=x.PropImgUrl1
            }).ToList();
        }
    }
}
