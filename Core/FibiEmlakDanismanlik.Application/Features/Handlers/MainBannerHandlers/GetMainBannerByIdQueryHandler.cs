using FibiEmlakDanismanlik.Application.Features.Queries.MainBannerQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MainBannerResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MainBannerHandlers
{
    public class GetMainBannerByIdQueryHandler : IRequestHandler<GetMainBannerByIdQuery, GetMainBannerByIdResult>
    {
        private readonly IRepository<GetMainBannerByIdResult> _repository;

        public GetMainBannerByIdQueryHandler(IRepository<GetMainBannerByIdResult> repository)
        {
            _repository = repository;
        }

        public async Task<GetMainBannerByIdResult> Handle(GetMainBannerByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);

            return new GetMainBannerByIdResult
            {
                MainBannerDesc = value.MainBannerDesc,
                MainBannerId = value.MainBannerId,
                MainBannerUrl = value.MainBannerUrl,
            };
        }
    }
}
