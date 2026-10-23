using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries
{
    /// <summary>
    /// Yönetim paneli ilan arayüzleri için olanaklar, kategoriler, aktif danışmanlar ve durumları getiren MediatR sorgusu.
    /// </summary>
    public class GetAdminListingLookupsQuery : IRequest<GetAdminListingLookupsResult>
    {
    }
}
