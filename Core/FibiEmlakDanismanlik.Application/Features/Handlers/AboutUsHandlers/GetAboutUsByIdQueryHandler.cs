using FibiEmlakDanismanlik.Application.Features.Queries.AboutUsQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AboutUsResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AboutUsHandlers
{
    public class GetAboutUsByIdQueryHandler : IRequestHandler<GetAboutUsByIdQuery, GetAboutUsByIdResult>
    {
        private readonly IRepository<AboutUs> _repository;

        public GetAboutUsByIdQueryHandler(IRepository<AboutUs> repository)
        {
            _repository = repository;
        }

        public async Task<GetAboutUsByIdResult> Handle(GetAboutUsByIdQuery request, CancellationToken cancellationToken)
        {
           var value = await _repository.GetByIdAsync(request.Id);
            return new GetAboutUsByIdResult
            {
                AboutUsId = value.AboutUsId,
                AboutUsDesc = value.AboutUsDesc,
                AboutUsImgUrl = value.AboutUsImgUrl,
                AboutUsTitle = value.AboutUsTitle,
            };
        }
    }
}
