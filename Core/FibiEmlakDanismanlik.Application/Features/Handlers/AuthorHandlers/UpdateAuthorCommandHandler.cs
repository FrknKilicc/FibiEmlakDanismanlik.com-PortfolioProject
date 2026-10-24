using FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthorHandlers
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
    {
        private readonly IRepository<Author> _repository;

        public UpdateAuthorCommandHandler(IRepository<Author> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.AuthorId);
            if (value == null)
            {
                throw new KeyNotFoundException("Yazar bulunamadı.");
            }

            value.AuthorName = request.AuthorName?.Trim() ?? string.Empty;
            value.AuthorImgUrl = request.AuthorImgUrl?.Trim() ?? string.Empty;

            await _repository.UpdateAsync(value);
        }
    }
}
