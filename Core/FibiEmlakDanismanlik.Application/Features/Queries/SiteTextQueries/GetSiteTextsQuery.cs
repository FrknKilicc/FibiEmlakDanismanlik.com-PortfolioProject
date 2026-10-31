using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.SiteTextResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.SiteTextQueries
{
    /// <summary>
    /// Site metinlerini (varsayılanlarla birleştirilmiş) anahtar → değer olarak getiren sorgu.
    /// </summary>
    public class GetSiteTextsQuery : IRequest<Dictionary<string, string>>
    {
    }

    /// <summary>
    /// Panel düzenleme ekranı için tanımlarla birlikte site metinlerini getiren sorgu.
    /// </summary>
    public class GetAdminSiteTextsQuery : IRequest<List<GetAdminSiteTextResult>>
    {
    }
}
