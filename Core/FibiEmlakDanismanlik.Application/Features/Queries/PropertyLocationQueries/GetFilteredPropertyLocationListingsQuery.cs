using FibiEmlakDanismanlik.Application.Features.Results.PropertyLocationResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.PropertyLocationQueries
{
    public record GetFilteredPropertyLocationListingsQuery(
        int UsageType,
        int ListingTypeId,
        string? Location,
        string? Keyword
    ) : IRequest<GetFilteredPropertyLocationListingsResponse>;
}
