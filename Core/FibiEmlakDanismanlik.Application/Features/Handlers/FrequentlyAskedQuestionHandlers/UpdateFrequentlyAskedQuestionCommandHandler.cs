using FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands;
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
    public class UpdateFrequentlyAskedQuestionCommandHandler : IRequestHandler<UpdateFrequentlyAskedQuestionCommand>
    {
        private readonly IRepository<FrequentlyAskedQuestion> _repository;

        public UpdateFrequentlyAskedQuestionCommandHandler(IRepository<FrequentlyAskedQuestion> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateFrequentlyAskedQuestionCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.FrequentlyAskedQuestionId);
            value.Question = request.Question;
            value.Answer = request.Answer;
            await _repository.UpdateAsync(value);   
        }
    }
}
