using FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AuthorQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AuthorResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthorHandlers
{
    public class GetAuthorQueryHandler : IRequestHandler<GetAuthorQuery,List<GetAuthorResult>>
    {
        private readonly IRepository<Author> _repository;

        public GetAuthorQueryHandler(IRepository<Author> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAuthorResult>> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x => new GetAuthorResult
            {
                AuthorId = x.AuthorId,
                AuthorImgUrl = x.AuthorImgUrl,
                AuthorName = x.AuthorName,
            }).ToList();
        }
    }
}
