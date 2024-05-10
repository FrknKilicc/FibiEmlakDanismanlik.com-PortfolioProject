using FibiEmlakDanismanlik.Application.Features.Queries.MainBannerQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MainBannerResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MainBannerHandlers
{
    public class GetMainBannerQueryHandler : IRequestHandler<GetMainBannerQuery, List<GetMainBannerResult>>
    {
        private readonly IRepository<MainBanner> _repository;

        public GetMainBannerQueryHandler(IRepository<MainBanner> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetMainBannerResult>> Handle(GetMainBannerQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x=> new GetMainBannerResult
            {
                MainBannerDesc=x.MainBannerDesc,
                MainBannerId=x.MainBannerId,
                MainBannerUrl=x.MainBannerUrl
            }).ToList();
        }
    }
}
