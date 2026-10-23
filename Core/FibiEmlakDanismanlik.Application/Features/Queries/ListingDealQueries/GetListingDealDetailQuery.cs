using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries
{
    /// <summary>
    /// Belirtilen kimlikteki işlemin tüm detayını ve prim paylarını getiren sorgu.
    /// </summary>
    public class GetListingDealDetailQuery : IRequest<ListingDealDetail?>
    {
        public int Id { get; set; }

        public GetListingDealDetailQuery()
        {
        }

        public GetListingDealDetailQuery(int id)
        {
            Id = id;
        }
    }
}
