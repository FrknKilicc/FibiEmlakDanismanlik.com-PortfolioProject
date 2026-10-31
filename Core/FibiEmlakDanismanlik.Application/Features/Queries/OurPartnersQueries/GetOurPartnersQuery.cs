using FibiEmlakDanismanlik.Application.Features.Results.OurPartnersResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries
{
    public class GetOurPartnersQuery : IRequest<List<GetOurPartnersResult>>
    {
        public bool IncludeInactive { get; set; }

        public GetOurPartnersQuery()
        {
        }

        public GetOurPartnersQuery(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }
    }
}
