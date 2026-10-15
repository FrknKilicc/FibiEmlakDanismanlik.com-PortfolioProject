using FibiEmlakDanismanlik.Application.Features.Results.HomeSectionImageResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.HomeSectionImageQueries
{
    public class GetHomeSectionImageByIdQuery : IRequest<GetHomeSectionImageByIdResult?>
    {
        public int Id { get; set; }

        public GetHomeSectionImageByIdQuery(int id)
        {
            Id = id;
        }
    }
}
