using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries
{
    /// <summary>
    /// Yönetim paneli ilanlarını filtreleyerek sayfalı olarak getiren MediatR sorgusu.
    /// </summary>
    public class GetAdminListingsQuery : IRequest<GetAdminListingsResult>
    {
        public bool? IsRent { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public int? AgentId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = AdminListingConstants.DefaultPageSize;
        public string? Sort { get; set; }
    }
}
