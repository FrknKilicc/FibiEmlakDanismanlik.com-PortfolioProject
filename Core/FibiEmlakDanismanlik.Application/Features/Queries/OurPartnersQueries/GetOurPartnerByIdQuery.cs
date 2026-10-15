using FibiEmlakDanismanlik.Application.Features.Results.OurPartnersResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries
{
    public class GetOurPartnerByIdQuery : IRequest<GetOurPartnerByIdResult?>
    {
        public int Id { get; set; }

        public GetOurPartnerByIdQuery(int id)
        {
            Id = id;
        }
    }
}
