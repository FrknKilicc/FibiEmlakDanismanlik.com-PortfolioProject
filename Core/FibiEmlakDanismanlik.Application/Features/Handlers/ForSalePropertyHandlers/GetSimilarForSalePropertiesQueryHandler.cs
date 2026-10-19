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

            var candidatesWithLevel = new List<(SimilarPropertyCandidateResult Candidate, int Level)>();
            var seenListingIds = new HashSet<int>();

            for (int level = 0; level <= 3; level++)
            {
                var candidates = await _propertyRepository.GetForSaleSimilarCandidatesAsync(reference, 24, level);
                if (candidates != null && candidates.Count > 0)
                {
                    foreach (var candidate in candidates)
                    {
                        if (seenListingIds.Add(candidate.ListingId))
                        {
                            candidatesWithLevel.Add((candidate, level));
                        }
                    }
                }

                if (candidatesWithLevel.Count >= 6)
                {
                    break;
                }
            }

            if (!candidatesWithLevel.Any())
                return new List<SimilarPropertyItemResult>();

            var result = candidatesWithLevel
                .Select(x => new
                {
                    x.Candidate,
                    x.Level,
                    Score = SimilarPropertyScorer.Calculate(reference, x.Candidate)
                })
                .OrderBy(x => x.Level)
                .ThenByDescending(x => x.Score)
                .ThenByDescending(x => x.Candidate.CreatedDate)
                .Take(6)
                .Select(x => new SimilarPropertyItemResult
                {
                    ListingId = x.Candidate.ListingId,
                    ListingTypeId = x.Candidate.ListingTypeId,
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
                    PropertyStatus = x.Candidate.PropertyStatus,
                    AgentImageUrl = x.Candidate.AgentImageUrl,
                    AgentName = x.Candidate.AgentName
                })
                .ToList();

            return result;
        }
    }
}
