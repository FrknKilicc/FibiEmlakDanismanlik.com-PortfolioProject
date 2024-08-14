using FibiEmlakDanismanlik.Application.Features.Results.ContactResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ContactQueries
{
    public class GetContactQuery:IRequest<List<GetContactResult>>
    {
    }
}
