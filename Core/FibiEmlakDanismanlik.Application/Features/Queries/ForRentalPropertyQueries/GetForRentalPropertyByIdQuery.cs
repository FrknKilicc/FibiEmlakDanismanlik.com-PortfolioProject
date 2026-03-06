using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries
{
    public class GetForRentalPropertyByIdQuery : IRequest<GetForRentalPropertyDetailResult>
    {
        public int Id { get; }
        public GetForRentalPropertyByIdQuery(int id) => Id = id;
    }
}
