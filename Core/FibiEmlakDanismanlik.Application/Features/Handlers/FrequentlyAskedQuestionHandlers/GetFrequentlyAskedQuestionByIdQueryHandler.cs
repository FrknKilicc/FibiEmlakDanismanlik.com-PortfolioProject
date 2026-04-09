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
    public class GetFrequentlyAskedQuestionByIdQueryHandler : IRequestHandler<GetFrequentlyAskedQuestionByIdQuery, GetFrequentlyAskedQuestionByIdResult>
    {
        private readonly IRepository<FrequentlyAskedQuestion> _repository;

        public GetFrequentlyAskedQuestionByIdQueryHandler(IRepository<FrequentlyAskedQuestion> repository)
        {
            _repository = repository;
        }

        public async Task<GetFrequentlyAskedQuestionByIdResult> Handle(GetFrequentlyAskedQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetFrequentlyAskedQuestionByIdResult
            {
                Answer = value.Answer,
                FrequentlyAskedQuestionId = request.Id,
                Question = value.Question,
            };
        }
    }
}
