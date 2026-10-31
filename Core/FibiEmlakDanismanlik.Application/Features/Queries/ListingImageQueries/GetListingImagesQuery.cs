using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.ListingImageResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingImageQueries
{
    /// <summary>
    /// Bir ilana ait tüm fotoğrafları listeleyen sorgu.
    /// </summary>
    public class GetListingImagesQuery : IRequest<List<GetListingImageResult>>
    {
        /// <summary>
        /// İlan türü kimliği.
        /// </summary>
        public int ListingTypeId { get; set; }

        /// <summary>
        /// İlan kimliği.
        /// </summary>
        public int ListingId { get; set; }

        public GetListingImagesQuery()
        {
        }

        public GetListingImagesQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
