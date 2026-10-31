using FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.PopularPlaceQueries
{
    public class GetPopularPlacesQuery : IRequest<GetPopularPlaceResult>
    {
        public int UsageType { get; set; }
        public int Take { get; set; }

        public GetPopularPlacesQuery(int usageType, int take)
        {
            UsageType = usageType;
            Take = take;
        }

        public GetPopularPlacesQuery()
        {
        }
    }
}
