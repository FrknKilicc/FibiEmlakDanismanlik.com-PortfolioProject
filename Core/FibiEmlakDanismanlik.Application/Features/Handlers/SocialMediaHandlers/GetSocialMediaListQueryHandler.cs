using FibiEmlakDanismanlik.Application.Features.Queries.SocialMediaQueries;
using FibiEmlakDanismanlik.Application.Features.Results.SocialMediaResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.SocialMediaHandlers
{
    public class GetSocialMediaListQueryHandler : IRequestHandler<GetSocialMediaListQuery, List<GetSocialMediaResult>>
    {
        private readonly IRepository<SocialMedia> _repository;

        public GetSocialMediaListQueryHandler(IRepository<SocialMedia> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetSocialMediaResult>> Handle(GetSocialMediaListQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            return value.Select(x => new GetSocialMediaResult
            {
                CreatedDate = DateTime.Now,
                Icon = x.Icon,
              Id = x.Id,  
              IsActive = x.IsActive,
              Name = x.Name,
              Order = x.Order,
              Url=x.Url,

            }).ToList();
        }
    }
}
