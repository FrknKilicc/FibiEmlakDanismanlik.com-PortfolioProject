using FibiEmlakDanismanlik.Application.Features.Results.MeResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MeQueries
{
    /// <summary>
    /// Giriş yapmış çalışanın profil ve hesap bilgilerini getiren sorgu.
    /// </summary>
    public class GetMyProfileQuery : IRequest<GetMyProfileResult>
    {
    }
}
