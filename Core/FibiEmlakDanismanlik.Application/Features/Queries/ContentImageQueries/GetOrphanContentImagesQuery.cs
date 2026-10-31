using FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ContentImageQueries
{
    /// <summary>
    /// Sitede hiçbir kayıtta kullanılmayan yetim içerik görsellerini listeleyen sorgu.
    /// </summary>
    public sealed class GetOrphanContentImagesQuery : IRequest<GetOrphanContentImagesResult>
    {
    }
}
