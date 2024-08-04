using FibiEmlakDanismanlik.Application.Features.Queries.RentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.RentalPropertyHandlers
{
    public class GetLast10RentalQueryHandler : IRequestHandler<GetLast10RentalPropertyQuery, List<RentalPropertyBaseViewModel>>
    {
        private readonly IPropertyRepository _repository;

        public GetLast10RentalQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RentalPropertyBaseViewModel>> Handle(GetLast10RentalPropertyQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_repository.GetAllRentalPropertyWithAgent());
        }
    }
}
