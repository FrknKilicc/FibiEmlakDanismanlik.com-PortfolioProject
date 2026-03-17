using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetSimilarForSalePropertiesQueryHandler : IRequestHandler<GetSimilarForSalePropertiesQuery, List<SimilarPropertyItemResult>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetSimilarForSalePropertiesQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<SimilarPropertyItemResult>> Handle(GetSimilarForSalePropertiesQuery request, CancellationToken cancellationToken)
        {
            var reference = await _propertyRepository.GetForSaleSimilarReferenceAsync(request.Id);

            if (reference == null)
                return new List<SimilarPropertyItemResult>();

            var candidates = await _propertyRepository.GetForSaleSimilarCandidatesAsync(reference, 24);

            if (candidates == null || !candidates.Any())
                return new List<SimilarPropertyItemResult>();

            var result = candidates
                .Select(candidate => new
                {
                    Candidate = candidate,
                    Score = SimilarPropertyScorer.Calculate(reference, candidate)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Candidate.CreatedDate)
                .Take(6)
                .Select(x => new SimilarPropertyItemResult
                {
                    ListingId = x.Candidate.ListingId,
                    ListingType = x.Candidate.ListingType,
                    PropertyName = x.Candidate.PropertyName,
                    PropertyDescription = x.Candidate.PropertyDescription,
                    City = x.Candidate.City,
                    District = x.Candidate.District,
                    Neighborhood = x.Candidate.Neighborhood,
                    Price = x.Candidate.Price,
                    SquareMeter = x.Candidate.SquareMeter,
                    NumberOfRoom = x.Candidate.NumberOfRoom,
                    CoverImageUrl = x.Candidate.CoverImageUrl,
                    PropertyStatus=x.Candidate.PropertyStatus,
                    AgentImageUrl=x.Candidate.AgentImageUrl,
                    AgentName = x.Candidate.AgentName
                    
                })
                .ToList();

            return result;
        }
    }
}
