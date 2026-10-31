using FibiEmlakDanismanlik.Application.Features.Results.ListingDealResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries
{
    /// <summary>
    /// Filtrelere göre sayfalanmış işlem kayıtlarını ve durum özetini getiren sorgu.
    /// </summary>
    public class GetListingDealsQuery : ListingDealFilter, IRequest<GetListingDealsResult>
    {
    }
}
