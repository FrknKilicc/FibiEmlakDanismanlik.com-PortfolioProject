using FibiEmlakDanismanlik.Application.Features.Queries.HomeSectionImageQueries;
using FibiEmlakDanismanlik.Application.Features.Results.HomeSectionImageResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.HomeSectionImageHandlers
{
    public class GetHomeSectionImageQueryHandler : IRequestHandler<GetHomeSectionImageQuery, List<GetHomeSectionImageResult>>
    {
        private readonly IRepository<HomeSectionImage> _repository;

        public GetHomeSectionImageQueryHandler(IRepository<HomeSectionImage> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetHomeSectionImageResult>> Handle(GetHomeSectionImageQuery request, CancellationToken cancellationToken)
        {
            var images = await _repository.GetAllAsync();

            var query = images.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.SectionKey))
            {
                var filterKey = request.SectionKey.Trim();
                query = query.Where(x => string.Equals(x.SectionKey, filterKey, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderBy(x => x.SectionKey)
                .ThenBy(x => x.Slot)
                .Select(x => new GetHomeSectionImageResult
                {
                    HomeSectionImageId = x.HomeSectionImageId,
                    SectionKey = x.SectionKey,
                    Slot = x.Slot,
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    IsActive = x.IsActive
                })
                .ToList();
        }
    }
}
