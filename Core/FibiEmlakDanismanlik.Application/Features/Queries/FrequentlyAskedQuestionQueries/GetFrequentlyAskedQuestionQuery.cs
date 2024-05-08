using FibiEmlakDanismanlik.Application.Features.Results.FrequentlyAskedQuestionResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.FrequentlyAskedQuestionQueries
{
    public class GetFrequentlyAskedQuestionQuery:IRequest<List<GetFrequentlyAskedQuestionResult>>
    {

    }
}
