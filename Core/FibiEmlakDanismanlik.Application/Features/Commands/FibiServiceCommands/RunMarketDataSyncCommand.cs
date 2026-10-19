using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Commands.FibiServiceCommands
{
    /// <summary>ProviderKey boşsa tüm sağlayıcılar zorla çalışır; bilinmeyen anahtar için null.</summary>
    public record RunMarketDataSyncCommand(string? ProviderKey) : IRequest<List<RunMarketDataSyncResult>?>;
}
