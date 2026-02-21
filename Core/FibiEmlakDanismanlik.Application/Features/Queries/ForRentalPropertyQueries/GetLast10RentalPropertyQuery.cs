using FibiEmlakDanismanlik.Application.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries
{
    public class GetLast10RentalPropertyQuery:IRequest<List<RentalPropertyBaseViewModel>>
    {
    }
}
