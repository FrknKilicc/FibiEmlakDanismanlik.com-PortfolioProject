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
    public class CreateFrequentlyAskedQuestionCommandHandler : IRequestHandler<CreateFrequentlyAskedQuestionCommand>
    {
        private readonly IRepository<FrequentlyAskedQuestion> _repository;

        public CreateFrequentlyAskedQuestionCommandHandler(IRepository<FrequentlyAskedQuestion> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateFrequentlyAskedQuestionCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new FrequentlyAskedQuestion
            {
                Answer = request.Answer,
                Question = request.Question,
            });
        }
    }
}
