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
    public class GetCurrentAboutUsQueryHandler : IRequestHandler<GetCurrentAboutUsQuery, GetAboutUsByIdResult?>
    {
        private readonly IRepository<AboutUs> _repository;

        public GetCurrentAboutUsQueryHandler(IRepository<AboutUs> repository)
        {
            _repository = repository;
        }

        public async Task<GetAboutUsByIdResult?> Handle(GetCurrentAboutUsQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            var current = values.OrderBy(x => x.AboutUsId).FirstOrDefault();
            if (current == null)
            {
                return null;
            }

            return new GetAboutUsByIdResult
            {
                AboutUsId = current.AboutUsId,
                AboutUsDesc = current.AboutUsDesc,
                AboutUsImgUrl = current.AboutUsImgUrl,
                AboutUsTitle = current.AboutUsTitle
            };
        }
    }
}
