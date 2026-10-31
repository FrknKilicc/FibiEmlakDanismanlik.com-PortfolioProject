using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries
{
    /// <summary>
    /// Haber bekleyen talep ayrıntılarını, bekleyen eşleşmelerini ve işlem geçmişini getiren sorgu.
    /// </summary>
    public class GetListingAlertDetailQuery : IRequest<ListingAlertDetail?>
    {
        public int Id { get; set; }

        public GetListingAlertDetailQuery()
        {
        }

        public GetListingAlertDetailQuery(int id)
        {
            Id = id;
        }
    }
}
