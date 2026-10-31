using FibiEmlakDanismanlik.Application.Features.Queries.HomeSectionImageQueries;
using FibiEmlakDanismanlik.Application.Features.Results.HomeSectionImageResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.HomeSectionImageHandlers
{
    public class GetHomeSectionImageByIdQueryHandler : IRequestHandler<GetHomeSectionImageByIdQuery, GetHomeSectionImageByIdResult?>
    {
        private readonly IRepository<HomeSectionImage> _repository;

        public GetHomeSectionImageByIdQueryHandler(IRepository<HomeSectionImage> repository)
        {
            _repository = repository;
        }

        public async Task<GetHomeSectionImageByIdResult?> Handle(GetHomeSectionImageByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
            {
                return null;
            }

            return new GetHomeSectionImageByIdResult
            {
                HomeSectionImageId = value.HomeSectionImageId,
                SectionKey = value.SectionKey,
                Slot = value.Slot,
                ImageUrl = value.ImageUrl,
                AltText = value.AltText,
                IsActive = value.IsActive
            };
        }
    }
}
