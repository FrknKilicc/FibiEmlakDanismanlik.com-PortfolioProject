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
    public class RemoveFrequentlyAskedQuestionCommandHandler : IRequestHandler<RemoveFrequentlyAskedQuestionCommand>
    {
        private readonly IRepository<FrequentlyAskedQuestion> _repository;

        public RemoveFrequentlyAskedQuestionCommandHandler(IRepository<FrequentlyAskedQuestion> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveFrequentlyAskedQuestionCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            await _repository.RemoveAsync(value);
        }
    }
}
