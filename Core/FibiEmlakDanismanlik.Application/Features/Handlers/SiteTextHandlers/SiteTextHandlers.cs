using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.SiteTextCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.SiteTextQueries;
using FibiEmlakDanismanlik.Application.Features.Results.SiteTextResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.SiteTextInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.SiteTextHandlers
{
    /// <summary>
    /// Site metinlerini varsayılanlarla birleştirip döner (site tarafı).
    /// </summary>
    public class GetSiteTextsQueryHandler : IRequestHandler<GetSiteTextsQuery, Dictionary<string, string>>
    {
        private readonly ISiteTextRepository _repository;

        public GetSiteTextsQueryHandler(ISiteTextRepository repository)
        {
            _repository = repository;
        }

        public async Task<Dictionary<string, string>> Handle(GetSiteTextsQuery request, CancellationToken cancellationToken)
        {
            return SiteTextDefinitions.Merge(await _repository.GetAllAsync());
        }
    }

    /// <summary>
    /// Panel ekranı için tanım + güncel değer listesini döner.
    /// </summary>
    public class GetAdminSiteTextsQueryHandler : IRequestHandler<GetAdminSiteTextsQuery, List<GetAdminSiteTextResult>>
    {
        private readonly ISiteTextRepository _repository;

        public GetAdminSiteTextsQueryHandler(ISiteTextRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAdminSiteTextResult>> Handle(GetAdminSiteTextsQuery request, CancellationToken cancellationToken)
        {
            var values = SiteTextDefinitions.Merge(await _repository.GetAllAsync());
            return SiteTextDefinitions.All.Select(d => new GetAdminSiteTextResult
            {
                Key = d.Key,
                Group = d.Group,
                Label = d.Label,
                Value = values[d.Key],
                Default = d.Default,
                Kind = d.Kind,
                MaxLength = d.MaxLength,
                Min = d.Min,
                Max = d.Max,
                Help = d.Help
            }).ToList();
        }
    }

    /// <summary>
    /// Site metinlerini günceller (doğrulama UpdateSiteTextsCommandValidator'da).
    /// </summary>
    public class UpdateSiteTextsCommandHandler : IRequestHandler<UpdateSiteTextsCommand>
    {
        private readonly ISiteTextRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSiteTextsCommandHandler(ISiteTextRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateSiteTextsCommand request, CancellationToken cancellationToken)
        {
            var values = request.Values.ToDictionary(x => x.Key, x => (x.Value ?? string.Empty).Trim());
            await _repository.SaveAsync(values, _currentUserService.AgentId);
        }
    }
}
