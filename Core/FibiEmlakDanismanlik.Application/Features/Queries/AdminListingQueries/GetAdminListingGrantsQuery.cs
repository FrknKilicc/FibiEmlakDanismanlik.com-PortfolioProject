using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries
{
    /// <summary>
    /// Belirli bir ilana tanımlanmış özel erişim izinlerini getiren sorgu (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class GetAdminListingGrantsQuery : IRequest<List<GetAdminListingGrantResult>>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetAdminListingGrantsQuery()
        {
        }

        public GetAdminListingGrantsQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
