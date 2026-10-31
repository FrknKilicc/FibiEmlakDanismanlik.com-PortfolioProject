using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingImageQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingImageResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingImageHandlers
{
    /// <summary>
    /// Bir ilana ait görselleri kapak görseli önce ve SortOrder sırasına göre getiren sorgu işleyicisi.
    /// </summary>
    public class GetListingImagesQueryHandler : IRequestHandler<GetListingImagesQuery, List<GetListingImageResult>>
    {
        private readonly IListingImageRepository _listingImageRepository;

        public GetListingImagesQueryHandler(IListingImageRepository listingImageRepository)
        {
            _listingImageRepository = listingImageRepository;
        }

        public async Task<List<GetListingImageResult>> Handle(GetListingImagesQuery request, CancellationToken cancellationToken)
        {
            if (!ListingImageConstants.IsKnownListingType(request.ListingTypeId))
            {
                return new List<GetListingImageResult>();
            }

            var images = await _listingImageRepository.GetByListingAsync(request.ListingTypeId, request.ListingId);

            return images
                .OrderByDescending(x => x.IsCover)
                .ThenBy(x => x.SortOrder)
                .Select(x => new GetListingImageResult
                {
                    ListingImageId = x.ListingImageId,
                    Url = x.Url,
                    MediumUrl = x.MediumUrl,
                    ThumbUrl = x.ThumbUrl,
                    Width = x.Width,
                    Height = x.Height,
                    AltText = x.AltText,
                    IsCover = x.IsCover,
                    SortOrder = x.SortOrder,
                    Kind = (int)x.Kind,
                    Caption = x.Caption
                })
                .ToList();
        }
    }
}
