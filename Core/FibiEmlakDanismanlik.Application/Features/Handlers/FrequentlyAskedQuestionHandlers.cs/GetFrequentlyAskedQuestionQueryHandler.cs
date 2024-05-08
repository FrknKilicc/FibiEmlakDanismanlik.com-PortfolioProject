using FibiEmlakDanismanlik.Application.Features.Queries.FrequentlyAskedQuestionQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FrequentlyAskedQuestionResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FrequentlyAskedQuestionHandlers.cs
{
    public class GetFrequentlyAskedQuestionQueryHandler : IRequestHandler<GetFrequentlyAskedQuestionQuery, List<GetFrequentlyAskedQuestionResult>>
    {
        private readonly IRepository<FrequentlyAskedQuestion> _repository;

        public GetFrequentlyAskedQuestionQueryHandler(IRepository<FrequentlyAskedQuestion> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetFrequentlyAskedQuestionResult>> Handle(GetFrequentlyAskedQuestionQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x => new GetFrequentlyAskedQuestionResult
            {
                Answer = x.Answer,
                FrequentlyAskedQuestionId = x.FrequentlyAskedQuestionId,
                Question = x.Question,
            }).ToList();

        }
    }
}
