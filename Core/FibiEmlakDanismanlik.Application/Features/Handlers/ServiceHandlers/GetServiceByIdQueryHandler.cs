using FibiEmlakDanismanlik.Application.Features.Queries.ServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ServiceResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ServiceHandlers
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, GetServiceByIdResult>
    {
        private readonly IRepository<Service> _repository;

        public GetServiceByIdQueryHandler(IRepository<Service> repository)
        {
            _repository = repository;
        }

        public async Task<GetServiceByIdResult> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            return new GetServiceByIdResult
            {
                ServiceDesc = value.ServiceDesc,
                ServiceId = value.ServiceId,
                ServiceImgUrl = value.ServiceImgUrl,
                ServiceTitle = value.ServiceTitle,
            };
        }
    }
}
