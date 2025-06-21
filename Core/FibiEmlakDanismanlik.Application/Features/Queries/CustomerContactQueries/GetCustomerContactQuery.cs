using FibiEmlakDanismanlik.Application.Features.Results.CustomerContactResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.CustomerContactQueries
{
    public class GetCustomerContactQuery:IRequest<GetCustomerContactResult>
    {
    }
}
